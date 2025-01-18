using EcomApplication.Models;

namespace EcomApplication.Interfaces
{
    public interface ICartService
    {
        Cart GetCart(int userId);
        void AddToCart(int userId, int productId, int quantity);
        void RemoveFromCart(int userId, int productId);
        void ClearCart(int userId);
    }
}
