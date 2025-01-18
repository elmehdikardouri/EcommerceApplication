using EcomApplication.Models;

namespace EcomApplication.Interfaces
{
    public interface IOrderService
    {
        void CreateOrder(Order order);
        IEnumerable<Order> GetOrdersByUserId(int userId);
    }
}

