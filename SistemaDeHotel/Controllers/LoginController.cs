using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace SistemaDeHotel.Controllers
{
    public class LoginController : Controller
    {
        // Página inicial del Login
        public IActionResult Login(string username, string password)
        {
            if (username == "admin" && password == "root")
            {
                // Guardamos en la sesión el estado de autenticación
                HttpContext.Session.SetString("IsLoggedIn", "true");

                // Redirigimos a la página principal o a la que corresponda
                return RedirectToAction("Index", "Home");
            }
            else
            {
                // Si las credenciales son incorrectas, mostramos un error
                ViewBag.ErrorMessage = "Credenciales incorrectas";
                return View();
            }
        }


        // Acción de Login
        [HttpPost]
        public IActionResult Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                // Validar el usuario y la contraseña
                if (model.Username == "admin" && model.Password == "root")
                {
                    // Almacenar en la sesión que el usuario ha iniciado sesión
                    HttpContext.Session.SetString("IsLoggedIn", "true");

                    // Redirigir al inicio después de iniciar sesión
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    ModelState.AddModelError("", "Usuario o contraseña incorrectos.");
                }
            }
            return View(model);
        }

        // Acción para cerrar sesión
        public IActionResult Logout()
        {
            HttpContext.Session.Remove("IsLoggedIn");
            return RedirectToAction("Index", "Home");
        }
    }

    // Modelo para Login
    public class LoginViewModel
    {
        [Required]
        public string Username { get; set; }

        [Required]
        public string Password { get; set; }
    }
}
