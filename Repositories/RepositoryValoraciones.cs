using Microsoft.EntityFrameworkCore;
using ProyectoRestauranteC_.Data;
using ProyectoRestauranteC_.Models;

namespace ProyectoRestauranteC_.Repositories
{
    public class RepositoryValoraciones
    {
        private readonly RestauranteContext _context;

        public RepositoryValoraciones(RestauranteContext context)
        {
            _context = context;
        }

        public async Task<List<Valoracion>> GetValoracionesVisiblesAsync()
        {
            return await _context.Valoraciones
                .Include(v => v.Usuario)
                .Where(v => v.Visible)
                .OrderByDescending(v => v.Fecha)
                .ToListAsync();
        }

        public async Task CrearValoracionAsync(int usuarioId, int calificacion, string comentario)
        {
            var valoracion = new Valoracion
            {
                UsuarioId = usuarioId,
                Puntuacion = calificacion,
                Comentario = comentario,
                Fecha = DateTime.Now,
                Visible = true
            };

            _context.Valoraciones.Add(valoracion);
            await _context.SaveChangesAsync();
        }
    }
}