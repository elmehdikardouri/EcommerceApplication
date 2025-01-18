using EcomApplication.Data;
using EcomApplication.Interfaces;
using EcomApplication.Models;
using Microsoft.EntityFrameworkCore;

namespace EcomApplication.Services
{
    public class CartService : ICartService
    {
        private readonly EcommerceDbContext _dbContext;

        public CartService(EcommerceDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public Cart GetCart(int userId)
        {
            return _dbContext.Carts
                .Include(c => c.CartItems)
                .ThenInclude(ci => ci.Product)
                .FirstOrDefault(c => c.UserId == userId);
        }

        public void AddToCart(int userId, int productId, int quantity)
        {
            var cart = GetCart(userId);
            if (cart == null)
            {
                cart = new Cart
                {
                    UserId = userId,
                    CartItems = new List<CartItem>()
                };
                _dbContext.Carts.Add(cart);
            }

            var cartItem = cart.CartItems.FirstOrDefault(ci => ci.ProductId == productId);
            if (cartItem != null)
            {
                cartItem.Quantity += quantity;
            }
            else
            {
                cart.CartItems.Add(new CartItem
                {
                    ProductId = productId,
                    Quantity = quantity
                });
            }

            _dbContext.SaveChanges();
        }

        public void RemoveFromCart(int userId, int productId)
        {
            var cart = GetCart(userId);
            if (cart == null) return;

            var cartItem = cart.CartItems.FirstOrDefault(ci => ci.ProductId == productId);
            if (cartItem != null)
            {
                cart.CartItems.Remove(cartItem);
                _dbContext.SaveChanges();
            }
        }
        public void ClearCart(int userId)
        {
            var cart = _dbContext.Carts.FirstOrDefault(c => c.UserId == userId);
            if (cart != null)
            {
                _dbContext.CartItems.RemoveRange(cart.CartItems);  // Supprimer les éléments du panier
                _dbContext.SaveChanges();  // Sauvegarder les modifications dans la base de données
            }
        }
    }
}
