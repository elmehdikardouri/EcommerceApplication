using EcomApplication.Data;
using EcomApplication.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EcomApplication.Controllers
{
    public class CartController : Controller
    {
        private readonly EcommerceDbContext _context;

        public CartController(EcommerceDbContext context)
        {
            _context = context;
        }

        public IActionResult AddToCart(int id)
        {
            // Récupérer l'ID de l'utilisateur depuis la session
            var userId = HttpContext.Session.GetInt32("UserId");

            if (userId == null)
            {
                return RedirectToAction("Login", "Account"); // Rediriger vers la connexion si l'utilisateur n'est pas connecté
            }

            // Récupérer ou créer le panier pour l'utilisateur
            var cart = _context.Carts
                .Include(c => c.CartItems)
                .FirstOrDefault(c => c.UserId == userId.Value);

            if (cart == null)
            {
                cart = new Cart
                {
                    UserId = userId.Value,
                    CartItems = new List<CartItem>()
                };
                _context.Carts.Add(cart);
                _context.SaveChanges();
            }

            // Ajouter le produit au panier ou incrémenter la quantité
            var cartItem = cart.CartItems.FirstOrDefault(ci => ci.ProductId == id);
            if (cartItem != null)
            {
                cartItem.Quantity++;
            }
            else
            {
                cartItem = new CartItem
                {
                    CartId = cart.Id,
                    ProductId = id,
                    Quantity = 1
                };
                cart.CartItems.Add(cartItem);
            }

            _context.SaveChanges();
            return RedirectToAction("ViewCart");
        }

        public IActionResult ViewCart()
        {
            // Récupérer l'ID de l'utilisateur depuis la session
            var userId = HttpContext.Session.GetInt32("UserId");

            if (userId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            // Récupérer le panier de l'utilisateur ou un panier vide par défaut
            var cart = _context.Carts
                .Include(c => c.CartItems)
                .ThenInclude(ci => ci.Product)
                .FirstOrDefault(c => c.UserId == userId.Value);

            if (cart == null)
            {
                // Si le panier n'existe pas, on retourne un panier vide
                cart = new Cart
                {
                    UserId = userId.Value,
                    CartItems = new List<CartItem>()
                };
            }

            return View(cart);
        }
        public IActionResult RemoveFromCart(int id)
        {
            // Récupérer l'élément du panier à partir de son ID
            var cartItem = _context.CartItems.FirstOrDefault(ci => ci.Id == id);

            if (cartItem != null)
            {
                _context.CartItems.Remove(cartItem);
                _context.SaveChanges();
            }

            return RedirectToAction("ViewCart"); // Recharger la vue du panier
        }


    }
}
