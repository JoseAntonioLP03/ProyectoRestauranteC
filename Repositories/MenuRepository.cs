using ProyectoRestauranteC_.Data;
using ProyectoRestauranteC_.Models;
using Microsoft.EntityFrameworkCore;

namespace ProyectoRestauranteC_.Repositories
{
    public class MenuRepository : IMenuRepository
    {
        private readonly RestauranteContext context;

        public MenuRepository(RestauranteContext context)
        {
            this.context = context;
        }

        public async Task<IEnumerable<Categoria>> GetMenuCompletoAsync()
        {
            return await context.Categorias
                .Include(c => c.Productos) 
                .Where(c => c.Activo)
                .ToListAsync();
        }
    }
}
