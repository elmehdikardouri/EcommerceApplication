using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using EcomApplication.Data;
using EcomApplication.Models;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

public class AccountController : Controller
{
    private readonly EcommerceDbContext _context;

    public AccountController(EcommerceDbContext context)
    {
        _context = context;
    }

    [HttpPost]
    public async Task<IActionResult> Login(string username, string password)
    {
        var user = _context.Users.SingleOrDefault(u => u.Username == username && u.PasswordHash == password);

        if (user != null)
        {
           
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.Role, user.Role)
            };

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var authProperties = new AuthenticationProperties
            {
                IsPersistent = true
            };

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity), authProperties);

            HttpContext.Session.SetInt32("UserId", user.Id);
            if (user.Role == "Admin")
            {
                return RedirectToAction("Index", "Admin");
            }
            else if (user.Role == "Client")
            {
                return RedirectToAction("ClientGallery", "Client");
            }
        }

        // Si l'utilisateur n'est pas trouvé ou le mot de passe est incorrect
        ModelState.AddModelError("", "Nom d'utilisateur ou mot de passe incorrect");
        return View();
    }


    [HttpGet]
    public IActionResult Login()
    {
        return View();
    }
    [HttpGet]
    public IActionResult Register()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Register(User user)
    {
        if (ModelState.IsValid)
        {
            // Hash the password before saving (you should use a proper hashing method)
            user.PasswordHash = user.PasswordHash; // Replace with actual hashing

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            // Redirect to login page after successful registration
            return RedirectToAction("Login");
        }

        // If we got this far, something failed, redisplay form
        return View(user);
    }

    [HttpPost]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return RedirectToAction("Login", "Account");
    }
}