namespace ECommerceAPI.Models;

public static class OrderStatuses
{
    public const string AwaitingPayment = "AwaitingPayment";    // La reserva de stock existe y está esperando el pago.
    public const string Paid = "Paid";                          // Se confirmó el pago.
    public const string Preparing = "Preparing";
    public const string Shipped = "Shipped";
    public const string Delivered = "Delivered";
    public const string Cancelled = "Cancelled";
    public const string Expired = "Expired";                    // Nunca llegó el pago y se liberó la reserva.
}