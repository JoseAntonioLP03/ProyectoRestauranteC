using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProyectoRestauranteC_.Data;
using ProyectoRestauranteC_.Models;

namespace ProyectoRestauranteC_.Controllers
{
    public class ValoracionesController : Controller
    {
        private readonly RestauranteContext context;

        public ValoracionesController(RestauranteContext context)
        {
            this.context = context;
        }

        public async Task<IActionResult> Index()
        {
            var valoraciones = await context.Valoraciones
                .Include(v => v.Usuario)
                .Where(v => v.Visible)
                .OrderByDescending(v => v.Fecha)
                .ToListAsync();

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

            var valoracion = new Valoracion
            {
                UsuarioId = usuarioId,
                Puntuacion = calificacion,
                Comentario = comentario,
                Fecha = DateTime.Now,
                Visible = true
            };

            context.Valoraciones.Add(valoracion);
            await context.SaveChangesAsync();

            return Json(new { success = true, message = "Valoración guardada exitosamente" });
        }
    }
}
