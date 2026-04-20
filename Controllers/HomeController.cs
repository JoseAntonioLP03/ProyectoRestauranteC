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
        private readonly RepositoryHome repoHome;

        public HomeController(RepositoryUsuarios repoUsuarios, RepositoryHome repoHome)
        {
            this.repoUsuarios = repoUsuarios;
            this.repoHome = repoHome;
        }

        public async Task<IActionResult> Index()
        {
            // Obtener datos reales de la base de datos
            var totalProductos = await repoHome.GetTotalProductosAsync();
            var totalPedidos = await repoHome.GetTotalPedidosAsync();
            
            // Calcular valoración media
            var (valoracionMedia, totalValoraciones) = await repoHome.GetEstadisticasValoracionesAsync();

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
            var (total, actual, imagen) = await repoHome.GetImagenGaleriaAsync(index);

            if (total == 0)
            {
                return Json(new { total = 0, imagenes = new List<object>() });
            }

            return Json(new
            {
                total,
                actual,
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
