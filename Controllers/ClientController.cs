using EcomApplication.Data;
using EcomApplication.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EcomApplication.Controllers
{
    [Authorize(Roles = "Client")]
    public class ClientController : Controller
    {
        private readonly EcommerceDbContext _context;

        public ClientController(EcommerceDbContext context)
        {
            _context = context;
        }
        public IActionResult Filtre(string category)
        {
            IEnumerable<Product> products;

            if (string.IsNullOrEmpty(category))
            {
                // Si aucun filtre n'est appliqué, afficher tous les produits
                products = _context.Products.ToList();
            }
            else
            {
                // Si un filtre est appliqué, afficher uniquement les produits de cette catégorie
                products = _context.Products.Where(p => p.Category.Name.ToLower() == category.ToLower()).ToList();
            }

            return View(products);
        }

        public IActionResult Index()
        {
            // Logique spécifique au client, comme afficher les produits, les commandes en cours, etc.
            return View();
        }

        public IActionResult ClientGallery()
        {
            var product = _context.Products.ToList(); // Récupérer tous les produits depuis la base de données
            return View(product); // Passer la liste des produits à la vue ClientGallery
        }

        public IActionResult ViewProducts()
        {
            // Retourner une vue pour afficher la liste des produits
            return View();
        }

        public IActionResult ViewOrderHistory()
        {
            // Retourner une vue pour afficher l'historique des commandes du client
            return View();
        }
    }
}
