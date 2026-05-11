using ProyectoRestauranteC_.Models;
using System.Net.Http.Json;

namespace ProyectoRestauranteC_.Repositories
{
    public class MenuRepository : IMenuRepository
    {
        private readonly IHttpClientFactory httpClientFactory;

        public MenuRepository(IHttpClientFactory httpClientFactory)
        {
            this.httpClientFactory = httpClientFactory;
        }

        public async Task<IEnumerable<Categoria>> GetMenuCompletoAsync()
        {
            var client = this.httpClientFactory.CreateClient("ApiTopMeal");
            return await client.GetFromJsonAsync<List<Categoria>>("api/Menu/MenuCompleto")
                   ?? new List<Categoria>();
        }
    }
}
