using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProyectoRestauranteC_.Models;
using ProyectoRestauranteC_.Repositories;
using System.Security.Claims;

namespace ProyectoRestauranteC_.Controllers
{
    [Authorize]
    public class ReservasController : Controller
    {
        private readonly RepositoryReservas _repoReservas;

        public ReservasController(RepositoryReservas repoReservas)
        {
            _repoReservas = repoReservas;
        }

        public async Task<IActionResult> Index()
        {
            var mesas = await _repoReservas.GetAllMesasAsync();
            return View(mesas);
        }

        [HttpPost]
        public async Task<IActionResult> ComprobarDisponibilidad(DateTime fecha, TimeSpan hora, int npersonas)
        {
            DateTime fechaTurno = fecha.Date.Add(hora);
            var mesasLibres = await _repoReservas.GetMesasDisponiblesAsync(fechaTurno, npersonas);
            
            // Asignación automática: ordenamos por capacidad para no desperdiciar mesas grandes en reservas pequeñas
            var mejorMesa = mesasLibres.OrderBy(m => m.Capacidad).FirstOrDefault();

            if (mejorMesa != null)
            {
                return Json(new { 
                    disponible = true, 
                    mesaId = mejorMesa.Id, 
                    capacidad = mejorMesa.Capacidad 
                });
            }

            return Json(new { disponible = false });
        }

        [HttpPost]
        public async Task<IActionResult> Reservar(int idMesa, DateTime fecha, TimeSpan hora, int npersonas)
        {
            int idUsuario = int.Parse(User.FindFirstValue("ID_USUARIO")!);
            DateTime fechaTurno = fecha.Date.Add(hora);
            
            await _repoReservas.CrearReservaAsync(idUsuario, idMesa, fechaTurno, npersonas);
            return Json(new { success = true });
        }
    }
}