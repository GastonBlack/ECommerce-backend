using ECommerceAPI.Data;
using ECommerceAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace ECommerceAPI.Services.Payments;

public class ExpiredReservationsCleanupService : BackgroundService
{
    // ======================================================================
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<ExpiredReservationsCleanupService> _logger;

    public ExpiredReservationsCleanupService(IServiceScopeFactory scopeFactory, ILogger<ExpiredReservationsCleanupService> logger)
    {
        _scopeFactory = scopeFactory;
        _logger = logger;
    }
    // ======================================================================

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                using var scope = _scopeFactory.CreateScope();
                var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

                var expiredOrders = await db.Orders
                    .Include(o => o.Items)
                    .Where(o =>
                        o.Status == OrderStatuses.AwaitingPayment &&
                        o.ReservationExpiresAt != null &&
                        o.ReservationExpiresAt < DateTime.UtcNow)
                    .ToListAsync(stoppingToken);

                foreach (var order in expiredOrders)
                {
                    await using var tx = await db.Database.BeginTransactionAsync(stoppingToken);

                    try
                    {
                        foreach (var item in order.Items)
                        {
                            await db.Database.ExecuteSqlInterpolatedAsync($@"
                                UPDATE ""Products""
                                SET ""ReservedStock"" = ""ReservedStock"" - {item.Quantity}
                                WHERE ""Id"" = {item.ProductId}
                                  AND ""ReservedStock"" >= {item.Quantity};
                            ", stoppingToken);
                        }

                        order.Status = OrderStatuses.Expired;
                        order.ReservationExpiresAt = null;

                        await db.SaveChangesAsync(stoppingToken);
                        await tx.CommitAsync(stoppingToken);

                        _logger.LogInformation("Reserva expirada y liberada. orderId={OrderId}", order.Id);
                    }
                    catch (Exception ex)
                    {
                        await tx.RollbackAsync(stoppingToken);
                        _logger.LogError(ex, "Error al liberar reserva vencida. orderId={OrderId}", order.Id);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error general en cleanup de reservas vencidas.");
            }

            await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
        }
    }
}