using Microsoft.EntityFrameworkCore;
using ProyectoRestauranteC_.Data;
using ProyectoRestauranteC_.Models;

namespace ProyectoRestauranteC_.Repositories
{
    public class RepositoryReservas
    {
        private readonly RestauranteContext context;

        public RepositoryReservas(RestauranteContext context)
        {
            this.context = context;
        }

        public async Task<List<Mesa>> GetMesasDisponiblesAsync(DateTime fechaTurno, int numPersonas)
        {
            // Mesas activas que tengan capacidad suficiente
            var mesasValidas = await this.context.Mesas
                .Where(m => m.Activa && m.Capacidad >= numPersonas)
                .ToListAsync();

            // Tiempo estimado de reserva: 90 minutos
            var inicioMargen = fechaTurno.AddMinutes(-89);
            var finMargen = fechaTurno.AddMinutes(89);

            // Reservas confirmadas o pendientes para la fecha y turno indicados
            var reservasEnTurno = await this.context.Reservas
                .Where(r => r.Estado != "Cancelada" && r.Estado != "Expirada")
                .Where(r => r.FechaReserva >= inicioMargen && r.FechaReserva <= finMargen)
                .Select(r => r.MesaId)
                .ToListAsync();

            // Filtramos las mesas validas quitando las que ya tienen reserva en ese turno
            var mesasDisponibles = mesasValidas.Where(m => !reservasEnTurno.Contains(m.Id)).ToList();

            return mesasDisponibles;
        }

        public async Task<List<Mesa>> GetAllMesasAsync()
        {
            return await this.context.Mesas.Where(m => m.Activa).ToListAsync();
        }
        
        public async Task<List<int>> GetMesasOcupadasEnTurnoAsync(DateTime fechaTurno)
        {
            // Tiempo estimado de reserva: 90 minutos
            var inicioMargen = fechaTurno.AddMinutes(-89);
            var finMargen = fechaTurno.AddMinutes(89);

            return await this.context.Reservas
                .Where(r => r.Estado != "Cancelada" && r.Estado != "Expirada")
                .Where(r => r.FechaReserva >= inicioMargen && r.FechaReserva <= finMargen)
                .Select(r => r.MesaId)
                .ToListAsync();
        }

        public async Task<Reserva> CrearReservaAsync(int usuarioId, int mesaId, DateTime fechaTurno, int numPersonas)
        {
            Reserva reserva = new Reserva
            {
                UsuarioId = usuarioId,
                MesaId = mesaId,
                FechaReserva = fechaTurno,
                NumeroPersonas = numPersonas,
                Estado = "Confirmada", // O "Pendiente" según tu lógica de negocio
                FechaCreacion = DateTime.Now,
                FechaConfirmacion = DateTime.Now
            };

            this.context.Reservas.Add(reserva);
            await this.context.SaveChangesAsync();

            return reserva;
        }

        public async Task<List<Reserva>> GetReservasByUsuarioAsync(int usuarioId)
        {
            return await this.context.Reservas
                .Include(r => r.Mesa)
                .Where(r => r.UsuarioId == usuarioId)
                .OrderByDescending(r => r.FechaReserva)
                .ToListAsync();
        }
        
        public async Task CancelarReservaAsync(int reservaId, int usuarioId)
        {
            var reserva = await this.context.Reservas
                .FirstOrDefaultAsync(r => r.Id == reservaId && r.UsuarioId == usuarioId);
                
            if (reserva != null)
            {
                reserva.Estado = "Cancelada";
                await this.context.SaveChangesAsync();
            }
        }
    }
}
