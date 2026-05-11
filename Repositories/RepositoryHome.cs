using System.Net.Http.Json;
using System.Text.Json;

namespace ProyectoRestauranteC_.Repositories
{
    public class RepositoryHome
    {
        private readonly IHttpClientFactory httpClientFactory;

        public RepositoryHome(IHttpClientFactory httpClientFactory)
        {
            this.httpClientFactory = httpClientFactory;
        }

        public async Task<int> GetTotalProductosAsync()
        {
            var client = this.httpClientFactory.CreateClient("ApiTopMeal");
            var response = await client.GetAsync("api/Home/TotalProductos");
            if (!response.IsSuccessStatusCode)
            {
                return 0;
            }

            return await response.Content.ReadFromJsonAsync<int>();
        }

        public async Task<int> GetTotalPedidosAsync()
        {
            var client = this.httpClientFactory.CreateClient("ApiTopMeal");
            var response = await client.GetAsync("api/Home/TotalPedidos");
            if (!response.IsSuccessStatusCode)
            {
                return 0;
            }

            return await response.Content.ReadFromJsonAsync<int>();
        }

        public async Task<(double ValoracionMedia, int TotalValoraciones)> GetEstadisticasValoracionesAsync()
        {
            var client = this.httpClientFactory.CreateClient("ApiTopMeal");
            var response = await client.GetAsync("api/Home/EstadisticasValoraciones");
            if (!response.IsSuccessStatusCode)
            {
                return (4.8, 0);
            }

            var payload = await response.Content.ReadFromJsonAsync<EstadisticasValoracionesResponse>();
            if (payload == null)
            {
                return (4.8, 0);
            }

            return (payload.ValoracionMedia, payload.TotalValoraciones);
        }

        public async Task<(int Total, int Actual, object? Imagen)> GetImagenGaleriaAsync(int index)
        {
            var client = this.httpClientFactory.CreateClient("ApiTopMeal");
            var response = await client.GetAsync($"api/Home/ImagenGaleria/{index}");
            if (!response.IsSuccessStatusCode)
            {
                return (0, 0, null);
            }

            var payload = await response.Content.ReadFromJsonAsync<ImagenGaleriaResponse>();
            if (payload == null)
            {
                return (0, 0, null);
            }

            object? imagen = payload.Imagen.ValueKind == JsonValueKind.Null
                || payload.Imagen.ValueKind == JsonValueKind.Undefined
                ? null
                : payload.Imagen;

            return (payload.Total, payload.Actual, imagen);
        }

        private class EstadisticasValoracionesResponse
        {
            public double ValoracionMedia { get; set; }
            public int TotalValoraciones { get; set; }
        }

        private class ImagenGaleriaResponse
        {
            public int Total { get; set; }
            public int Actual { get; set; }
            public JsonElement Imagen { get; set; }
        }
    }
}