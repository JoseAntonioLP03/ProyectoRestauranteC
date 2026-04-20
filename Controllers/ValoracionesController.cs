using Microsoft.AspNetCore.Mvc;
using ProyectoRestauranteC_.Repositories;
using ProyectoRestauranteC_.Models;

namespace ProyectoRestauranteC_.Controllers
{
    public class ValoracionesController : Controller
    {
        private readonly RepositoryValoraciones _repo;

        public ValoracionesController(RepositoryValoraciones repo)
        {
            _repo = repo;
        }

        public async Task<IActionResult> Index()
        {
            var valoraciones = await _repo.GetValoracionesVisiblesAsync();
            return View(valoraciones);
        }

        public IActionResult Crear()
        {
            if (!User.Identity!.IsAuthenticated)
            {
                return RedirectToAction("Login", "Acceso");
            }

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Crear(int calificacion, string comentario)
        {
            if (!User.Identity!.IsAuthenticated)
            {
                return Unauthorized(new { message = "Debes estar autenticado para dejar una valoración" });
            }

            if (calificacion < 1 || calificacion > 5)
            {
                return BadRequest(new { message = "La calificación debe estar entre 1 y 5" });
            }

            if (string.IsNullOrWhiteSpace(comentario) || comentario.Length > 500)
            {
                return BadRequest(new { message = "El comentario no puede estar vacío o exceder 500 caracteres" });
            }

            var usuarioId = int.Parse(User.FindFirst("ID_USUARIO")?.Value ?? "0");
            if (usuarioId == 0)
            {
                return Unauthorized(new { message = "Error al obtener tu información de usuario" });
            }

            await _repo.CrearValoracionAsync(usuarioId, calificacion, comentario);

            return Json(new { success = true, message = "Valoración guardada exitosamente" });
        }
    }
}
