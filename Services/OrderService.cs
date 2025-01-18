using EcomApplication.Data;
using EcomApplication.Interfaces;
using EcomApplication.Models;
using Microsoft.EntityFrameworkCore;

namespace EcomApplication.Services
{
    public class OrderService : IOrderService
    {
        private readonly EcommerceDbContext _dbContext;

        public OrderService(EcommerceDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public void CreateOrder(Order order)
        {
            _dbContext.Orders.Add(order);
            _dbContext.SaveChanges();
        }

        public IEnumerable<Order> GetOrdersByUserId(int userId)
        {
            return _dbContext.Orders
                .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Product)
                .Where(o => o.UserId == userId)
                .ToList();
        }
    }
}
