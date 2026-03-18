using System.Diagnostics;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using ProyectoRestauranteC_.Data;
using ProyectoRestauranteC_.Models;
using ProyectoRestauranteC_.Repositories;

namespace ProyectoRestauranteC_.Controllers
{
    public class HomeController : Controller
    {
        private readonly RepositoryUsuarios repoUsuarios;
        private readonly RestauranteContext context;

        public HomeController(RepositoryUsuarios repoUsuarios, RestauranteContext context)
        {
            this.repoUsuarios = repoUsuarios;
            this.context = context;
        }

        public async Task<IActionResult> Index()
        {
            // Obtener datos reales de la base de datos
            var totalProductos = await context.Productos.Where(p => p.Disponible).CountAsync();
            var totalPedidos = await context.Pedidos.CountAsync();
            
            // Calcular valoración media
            var valoraciones = await context.Valoraciones
                .Where(v => v.Visible)
                .ToListAsync();
            
            var valoracionMedia = valoraciones.Any() 
                ? Math.Round(valoraciones.Average(v => v.Puntuacion), 1) 
                : 4.8;
            
            var totalValoraciones = valoraciones.Count;

            var stats = new
            {
                TotalProductos = Math.Max(totalProductos, 30), // Mínimo 30
                TotalPedidos = Math.Max(totalPedidos, 100), // Mínimo 100
                ValoracionMedia = valoracionMedia,
                TotalValoraciones = Math.Max(totalValoraciones, 50), // Mínimo 50
                TiempoEntrega = 30 // En minutos
            };

            return View(stats);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        public async Task<IActionResult> CuponesDisponibles()
        {
            var cupones = await repoUsuarios.GetCuponesDisponiblesAsync();
            var result = cupones.Select(c => new
            {
                codigo = c.Codigo,
                tipoDescuento = c.TipoDescuento,
                valorDescuento = c.ValorDescuento,
                fechaFin = c.FechaFin.ToString("dd/MM/yyyy"),
                usosRestantes = c.UsoMaximo - c.VecesUsado
            });
            return Json(result);
        }

        [HttpGet]
        public async Task<IActionResult> ObtenerImagenesGaleria(int index = 0)
        {
            var total = await context.Galeria
                .Where(g => g.Activa)
                .CountAsync();

            if (total == 0)
            {
                return Json(new { total = 0, imagenes = new List<object>() });
            }

            if (index < 0)
            {
                index = total - 1;
            }
            else if (index >= total)
            {
                index = 0;
            }

            var imagen = await context.Galeria
                .Where(g => g.Activa)
                .OrderBy(g => g.Id)
                .Skip(index)
                .Select(g => new
                {
                    id = g.Id,
                    urlImagen = g.UrlImagen,
                    descripcion = g.Descripcion,
                    tipo = g.Tipo
                })
                .FirstOrDefaultAsync();

            return Json(new
            {
                total,
                actual = index,
                imagen
            });
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
