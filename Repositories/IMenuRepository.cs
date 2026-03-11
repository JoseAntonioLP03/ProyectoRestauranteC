using ProyectoRestauranteC_.Models;

namespace ProyectoRestauranteC_.Repositories
{
    public interface IMenuRepository
    {
        
        Task<IEnumerable<Categoria>> GetMenuCompletoAsync();
    }
}
