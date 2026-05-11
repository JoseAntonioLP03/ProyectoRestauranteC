using Microsoft.AspNetCore.Http;
using ProyectoRestauranteC_.Models;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace ProyectoRestauranteC_.Repositories
{
    public class RepositoryValoraciones
    {
        private readonly IHttpClientFactory httpClientFactory;
        private readonly IHttpContextAccessor httpContextAccessor;

        public RepositoryValoraciones(IHttpClientFactory httpClientFactory, IHttpContextAccessor httpContextAccessor)
        {
            this.httpClientFactory = httpClientFactory;
            this.httpContextAccessor = httpContextAccessor;
        }

        public async Task<List<Valoracion>> GetValoracionesVisiblesAsync()
        {
            var client = this.httpClientFactory.CreateClient("ApiTopMeal");
            return await client.GetFromJsonAsync<List<Valoracion>>("api/Valoraciones/ValoracionesVisibles")
                   ?? new List<Valoracion>();
        }

        public async Task CrearValoracionAsync(int usuarioId, int calificacion, string comentario)
        {
            var client = CreateAuthorizedClient();
            var response = await client.PostAsJsonAsync("api/Valoraciones/CrearValoracion", new
            {
                Calificacion = calificacion,
                Comentario = comentario
            });

            response.EnsureSuccessStatusCode();
        }

        private HttpClient CreateAuthorizedClient()
        {
            var client = this.httpClientFactory.CreateClient("ApiTopMeal");
            var token = this.httpContextAccessor.HttpContext?.Session.GetString("API_TOKEN");
            if (!string.IsNullOrWhiteSpace(token))
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }

            return client;
        }
    }
}