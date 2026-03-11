using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using ProyectoRestauranteC_.Models;
using ProyectoRestauranteC_.Repositories;

namespace ProyectoRestauranteC_.Controllers
{
    public class HomeController : Controller
    {
        private readonly RepositoryUsuarios repoUsuarios;

        public HomeController(RepositoryUsuarios repoUsuarios)
        {
            this.repoUsuarios = repoUsuarios;
        }

        public IActionResult Index()
        {
            return View();
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

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
