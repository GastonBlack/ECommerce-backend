using ECommerceAPI.Models;

namespace ECommerceAPI.Helpers;

public static class OrderStatusHelper
{
    public static bool CanTransition(string currentStatus, string newStatus)
    {
        return currentStatus switch
        {
            OrderStatuses.Pending => newStatus == OrderStatuses.Paid
                                  || newStatus == OrderStatuses.Cancelled,

            OrderStatuses.Paid => newStatus == OrderStatuses.Preparing
                               || newStatus == OrderStatuses.Cancelled,

            OrderStatuses.Preparing => newStatus == OrderStatuses.Shipped
                                    || newStatus == OrderStatuses.Cancelled,

            OrderStatuses.Shipped => newStatus == OrderStatuses.Delivered,

            OrderStatuses.Delivered => false,
            OrderStatuses.Cancelled => false,

            _ => false
        };
    }
}