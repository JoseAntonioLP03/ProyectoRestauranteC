using Microsoft.AspNetCore.Mvc;
using ProyectoRestauranteC_.Models;
using ProyectoRestauranteC_.Repositories;
using Microsoft.AspNetCore.Http; 

namespace ProyectoRestauranteC_.Controllers
{
    public class AccesoController : Controller
    {
        private RepositoryUsuarios repo;

        public AccesoController(RepositoryUsuarios repo)
        {
            this.repo = repo;
        }

        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(string email, string password)
        {
            Usuario? usuario = await this.repo.ExisteUsuarioAsync(email, password);

            if (usuario != null)
            {
                HttpContext.Session.SetInt32("ID_USUARIO", usuario.Id);
                HttpContext.Session.SetString("NOMBRE_USUARIO", usuario.Nombre);
                HttpContext.Session.SetString("ROL_USUARIO", usuario.Rol);

                return RedirectToAction("Index", "Home");
            }
            else
            {
                ViewData["MENSAJE"] = "Email o contraseña incorrectos";
                return View();
            }
        }

        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(string nombre, string email, string password, string telefono, string direccion)
        {
            await this.repo.RegisterUsuarioAsync(nombre, email, password, telefono, direccion);

            return RedirectToAction("Login");
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index", "Home");
        }
    }
}