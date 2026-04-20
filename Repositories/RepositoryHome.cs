using Microsoft.EntityFrameworkCore;
using ProyectoRestauranteC_.Data;

namespace ProyectoRestauranteC_.Repositories
{
    public class RepositoryHome
    {
        private readonly RestauranteContext _context;

        public RepositoryHome(RestauranteContext context)
        {
            _context = context;
        }

        public async Task<int> GetTotalProductosAsync()
        {
            return await _context.Productos.Where(p => p.Disponible).CountAsync();
        }

        public async Task<int> GetTotalPedidosAsync()
        {
            return await _context.Pedidos.CountAsync();
        }

        public async Task<(double ValoracionMedia, int TotalValoraciones)> GetEstadisticasValoracionesAsync()
        {
            var valoraciones = await _context.Valoraciones
                .Where(v => v.Visible)
                .ToListAsync();

            var valoracionMedia = valoraciones.Any() 
                ? Math.Round(valoraciones.Average(v => v.Puntuacion), 1) 
                : 4.8;

            return (valoracionMedia, valoraciones.Count);
        }

        public async Task<(int Total, int Actual, object? Imagen)> GetImagenGaleriaAsync(int index)
        {
            var total = await _context.Galeria
                .Where(g => g.Activa)
                .CountAsync();

            if (total == 0)
            {
                return (0, 0, null);
            }

            if (index < 0)
            {
                index = total - 1;
            }
            else if (index >= total)
            {
                index = 0;
            }

            var imagen = await _context.Galeria
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

            return (total, index, imagen);
        }
    }
}