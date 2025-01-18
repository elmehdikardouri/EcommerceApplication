using EcomApplication.Models;
using Microsoft.AspNetCore.Mvc;
using EcomApplication.Data;
using EcomApplication.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

public class OrderController : Controller
{
    private readonly IProductService _productService;
    private readonly ICartService _cartService;
    private readonly IOrderService _orderService;

    public OrderController(IProductService productService, ICartService cartService, IOrderService orderService)
    {
        _productService = productService;
        _cartService = cartService;
        _orderService = orderService;
    }

    // Passer commande
    public IActionResult Order(int? productId)  // Le paramètre productId est optionnel
    {
        // Vérification si un utilisateur est connecté
        var userId = HttpContext.Session.GetInt32("UserId");
        if (userId == null)
        {
            return RedirectToAction("Login", "Account");  // Redirige vers la page de connexion si l'utilisateur n'est pas authentifié
        }

        // Si un produit spécifique est commandé
        if (productId.HasValue)
        {
            var product = _productService.GetProductById(productId.Value);
            if (product == null)
            {
                return RedirectToAction("Index", "Client"); // Redirection si le produit n'existe pas
            }

            var order = new Order
            {
                UserId = userId.Value,
                OrderDate = DateTime.Now,
                TotalAmount = product.Price,
                OrderItems = new List<OrderItem>
                {
                    new OrderItem
                    {
                        ProductId = product.Id,
                        Quantity = 1,
                        UnitPrice = product.Price
                    }
                }
            };

            _orderService.CreateOrder(order);  // Créer la commande

            return View(order);  // Afficher la vue de confirmation avec les détails de la commande
        }

        // Si aucune commande spécifique n'est demandée, on passe à la commande depuis le panier
        var cart = _cartService.GetCart(userId.Value);
        if (cart == null || !cart.CartItems.Any())
        {
            return RedirectToAction("ClientGallery", "Client");  // Redirection si le panier est vide
        }

        // Créer une commande à partir du panier
        var orderFromCart = new Order
        {
            UserId = userId.Value,
            OrderDate = DateTime.Now,
            TotalAmount = cart.CartItems.Sum(ci => ci.Quantity * ci.Product.Price),
            OrderItems = cart.CartItems.Select(ci => new OrderItem
            {
                ProductId = ci.ProductId,
                Quantity = ci.Quantity,
                UnitPrice = ci.Product.Price
            }).ToList()
        };

        _orderService.CreateOrder(orderFromCart);  // Créer la commande depuis le panier

        // Vider le panier après la commande (optionnel)
        _cartService.ClearCart(userId.Value);

        return View(orderFromCart);  // Afficher la vue de confirmation avec les détails de la commande
    }
}
