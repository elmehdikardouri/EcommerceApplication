using EcomApplication.Data;
using EcomApplication.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EcomApplication.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {

        private readonly EcommerceDbContext _context;

        public AdminController(EcommerceDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            // Logique spécifique à l'admin, comme afficher les statistiques ou gérer les produits
            return View();
        }

        // D'autres actions administratives comme gérer les produits, les commandes, etc.

        // Afficher la liste des utilisateurs
        public IActionResult Users()
        {
            var users = _context.Users.ToList();
            return View(users);
        }
        // Afficher le formulaire pour ajouter un utilisateur (GET)
        [HttpGet]
        public IActionResult AddUser()
        {
            return View(new User());
        }

        // Ajouter un utilisateur (POST)
        [HttpPost]
        public IActionResult AddUser(User model)
        {
            if (ModelState.IsValid)
            {
                // Créer un nouvel utilisateur avec les données du modèle
                var user = new User
                {
                    Username = model.Username,
                    Email = model.Email,
                    PasswordHash = model.PasswordHash, // Assurez-vous de hacher le mot de passe avant de le sauvegarder !
                    Role = model.Role
                };

                // Ajouter l'utilisateur à la base de données
                _context.Users.Add(user);
                _context.SaveChanges();

                // Rediriger vers la liste des utilisateurs après l'ajout
                return RedirectToAction("Users");
            }

            // Si le modèle est invalide, retourner au formulaire avec les erreurs
            return View(model);
        }







        // Modifier un utilisateur (GET pour le formulaire)
        [HttpGet]
        public IActionResult EditUser(int id)
        {
            var user = _context.Users.Find(id);
            if (user == null)
            {
                return NotFound();
            }
            return View(user);
        }

        // Modifier un utilisateur (POST pour l'enregistrement)
        [HttpPost]
        public IActionResult EditUser(User model)
        {
            if (ModelState.IsValid)
            {
                // Trouver l'utilisateur à mettre à jour
                var user = _context.Users.FirstOrDefault(u => u.Id == model.Id);

                if (user != null)
                {
                    // Mise à jour des propriétés de l'utilisateur
                    user.Username = model.Username;
                    user.Email = model.Email;
                    user.Role = model.Role;
                    user.PasswordHash = model.PasswordHash;

                    // Sauvegarder les modifications dans la base de données
                    _context.SaveChanges();

                    // Rediriger vers la page des utilisateurs (ou un autre endroit après la mise à jour)
                    return RedirectToAction("Users");
                }
                else
                {
                    // Si l'utilisateur n'existe pas
                    return NotFound();
                }
            }

            // Si les données ne sont pas valides, retourner à la même page avec les erreurs de validation
            return View(model);
        }


        // Supprimer un utilisateur
        [HttpPost]
        public IActionResult DeleteUser(int id)
        {
            var user = _context.Users.Find(id);
            if (user != null)
            {
                _context.Users.Remove(user);
                _context.SaveChanges();
            }
            return RedirectToAction("Users");
        }

        // Afficher tous les produits
        public IActionResult Products()
        {
            var products = _context.Products.Include(p => p.Category).ToList();
            return View(products);
        }

        // GET: Product/AddProduct
        [HttpGet]
        public IActionResult AddProduct()
        {
            // Charger les catégories depuis la base de données pour les afficher dans le formulaire
            ViewBag.Categories = _context.Categories.ToList();
            return View();
        }

        // Ajouter un produit à la base de données
        [HttpPost]
        public IActionResult Create(IFormFile image)
        {
            // Récupérer les données du formulaire
            var name = Request.Form["Name"];
            var price = decimal.TryParse(Request.Form["Price"], out var parsedPrice) ? parsedPrice : 0;
            var description = Request.Form["Description"];
            var categoryId = int.TryParse(Request.Form["CategoryId"], out var parsedCategoryId) ? parsedCategoryId : 0;

            // Validation des données
            if (string.IsNullOrEmpty(name))
            {
                ModelState.AddModelError("Name", "Le nom du produit est obligatoire.");
            }
            if (price <= 0)
            {
                ModelState.AddModelError("Price", "Le prix doit être supérieur à 0.");
            }
            if (categoryId <= 0)
            {
                ModelState.AddModelError("CategoryId", "Une catégorie valide est obligatoire.");
            }

            // Si le formulaire contient des erreurs, renvoyer la vue avec les erreurs
            if (!ModelState.IsValid)
            {
                ViewBag.Categories = _context.Categories.ToList();
                return View();
            }

            // Gestion du fichier image
            byte[] imageData = null;
            if (image != null && image.Length > 0)
            {
                using (var memoryStream = new MemoryStream())
                {
                    image.CopyTo(memoryStream);
                    imageData = memoryStream.ToArray();
                }
            }

            // Créer un objet Product
            var product = new Product
            {
                Name = name,
                Price = price,
                Description = description,
                CategoryId = categoryId,
                Image = imageData
            };

            // Ajouter le produit à la base de données
            _context.Products.Add(product);
            _context.SaveChanges();

            // Rediriger vers la liste des produits ou une autre page
            return RedirectToAction("Products");
        }


        [HttpGet]
        public IActionResult Edit(int id)
        {
            // Récupérer le produit à modifier depuis la base de données
            var product = _context.Products.Find(id);
            if (product == null)
            {
                return NotFound();
            }

            // Charger les catégories depuis la base de données pour les afficher dans le formulaire
            ViewBag.Categories = _context.Categories.ToList();
            return View(product);
        }

        [HttpPost]
        public IActionResult Edit(int id, IFormFile image)
        {
            var product = _context.Products.Find(id);
            if (product == null)
            {
                return NotFound();
            }

            var name = Request.Form["Name"];
            var price = decimal.TryParse(Request.Form["Price"], out var parsedPrice) ? parsedPrice : 0;
            var description = Request.Form["Description"];
            var categoryId = int.TryParse(Request.Form["CategoryId"], out var parsedCategoryId) ? parsedCategoryId : 0;

            if (string.IsNullOrEmpty(name))
            {
                ModelState.AddModelError("Name", "Le nom du produit est obligatoire.");
            }
            if (price <= 0)
            {
                ModelState.AddModelError("Price", "Le prix doit être supérieur à 0.");
            }
            if (categoryId <= 0)
            {
                ModelState.AddModelError("CategoryId", "Une catégorie valide est obligatoire.");
            }

            if (!ModelState.IsValid)
            {
                ViewBag.Categories = _context.Categories.ToList();
                return View(product);
            }

            if (image != null && image.Length > 0)
            {
                using (var memoryStream = new MemoryStream())
                {
                    image.CopyTo(memoryStream);
                    product.Image = memoryStream.ToArray();
                }
            }
            else
            {
                var currentImage = Request.Form["CurrentImage"];
                if (!string.IsNullOrEmpty(currentImage))
                {
                    product.Image = Convert.FromBase64String(currentImage);
                }
            }

            product.Name = name;
            product.Price = price;
            product.Description = description;
            product.CategoryId = categoryId;

            _context.SaveChanges();

            return RedirectToAction("Products");
        }



        // Supprimer un produit
        [HttpPost]
        public IActionResult DeleteProduct(int id)
        {
            var product = _context.Products.Find(id);
            if (product != null)
            {
                _context.Products.Remove(product);
                _context.SaveChanges();
            }
            return RedirectToAction("Products");
        }

        public IActionResult Orders()
        {
            var orders = _context.Orders
                .Include(o => o.User) // Inclure l'utilisateur associé
                .Include(o => o.OrderItems) // Inclure les items de la commande
                .ThenInclude(oi => oi.Product) // Inclure les produits associés aux items
                .ToList();

            return View(orders);
        }


        // Détails d'une commande spécifique
        public IActionResult OrderDetails(int id)
        {
            var order = _context.Orders
                .Include(o => o.User) // Inclure l'utilisateur
                .Include(o => o.OrderItems) // Inclure les items de la commande
                .ThenInclude(oi => oi.Product) // Inclure les produits associés
                .FirstOrDefault(o => o.Id == id); // Rechercher la commande par ID

            if (order == null)
            {
                return NotFound(); // Retourner une erreur si la commande n'existe pas
            }

            return View(order);
        }

        public IActionResult EditOrder(int id)
        {
            var existingOrder = _context.Orders
                .Include(o => o.OrderItems)
                .FirstOrDefault(o => o.Id == id);

            if (existingOrder == null)
            {
                return NotFound();
            }

            // Convertir OrderItems en List pour faciliter l'accès par index
            existingOrder.OrderItems = existingOrder.OrderItems.ToList();

            return View(existingOrder);
        }




        // Supprimer une commande
        [HttpPost]
        public IActionResult DeleteOrder(int id)
        {
            var order = _context.Orders
                .Include(o => o.OrderItems) // Inclure les items pour les supprimer aussi
                .FirstOrDefault(o => o.Id == id);

            if (order != null)
            {
                // Supprimer les items associés à la commande
                _context.OrderItems.RemoveRange(order.OrderItems);

                // Supprimer la commande elle-même
                _context.Orders.Remove(order);
                _context.SaveChanges();
            }

            return RedirectToAction("Orders");
        }


    }


}
