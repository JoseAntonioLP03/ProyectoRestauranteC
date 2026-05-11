using Microsoft.AspNetCore.Http;
using ProyectoRestauranteC_.Models;
using System.Globalization;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace ProyectoRestauranteC_.Repositories
{
    public class RepositoryReservas
    {
        private readonly IHttpClientFactory httpClientFactory;
        private readonly IHttpContextAccessor httpContextAccessor;

        public RepositoryReservas(IHttpClientFactory httpClientFactory, IHttpContextAccessor httpContextAccessor)
        {
            this.httpClientFactory = httpClientFactory;
            this.httpContextAccessor = httpContextAccessor;
        }

        public async Task<List<Mesa>> GetMesasDisponiblesAsync(DateTime fechaTurno, int numPersonas)
        {
            var client = this.httpClientFactory.CreateClient("ApiTopMeal");
            var fechaParam = Uri.EscapeDataString(fechaTurno.ToString("o", CultureInfo.InvariantCulture));
            return await client.GetFromJsonAsync<List<Mesa>>(
                       $"api/Reservas/MesasDisponibles/{fechaParam}/{numPersonas}")
                   ?? new List<Mesa>();
        }

        public async Task<List<Mesa>> GetAllMesasAsync()
        {
            var client = this.httpClientFactory.CreateClient("ApiTopMeal");
            return await client.GetFromJsonAsync<List<Mesa>>("api/Reservas/Mesas")
                   ?? new List<Mesa>();
        }
        
        public async Task<List<int>> GetMesasOcupadasEnTurnoAsync(DateTime fechaTurno)
        {
            var client = this.httpClientFactory.CreateClient("ApiTopMeal");
            var fechaParam = Uri.EscapeDataString(fechaTurno.ToString("o", CultureInfo.InvariantCulture));
            return await client.GetFromJsonAsync<List<int>>(
                       $"api/Reservas/MesasOcupadas/{fechaParam}")
                   ?? new List<int>();
        }

        public async Task<Reserva> CrearReservaAsync(int usuarioId, int mesaId, DateTime fechaTurno, int numPersonas)
        {
            var client = CreateAuthorizedClient();
            var response = await client.PostAsJsonAsync("api/Reservas/CrearReserva", new
            {
                MesaId = mesaId,
                FechaTurno = fechaTurno,
                NumPersonas = numPersonas
            });

            response.EnsureSuccessStatusCode();
            return (await response.Content.ReadFromJsonAsync<Reserva>())!;
        }

        public async Task<List<Reserva>> GetReservasByUsuarioAsync(int usuarioId)
        {
            var client = CreateAuthorizedClient();
            var response = await client.GetAsync("api/Reservas/ReservasUsuario");
            if (!response.IsSuccessStatusCode)
            {
                return new List<Reserva>();
            }

            return await response.Content.ReadFromJsonAsync<List<Reserva>>() ?? new List<Reserva>();
        }
        
        public async Task CancelarReservaAsync(int reservaId, int usuarioId)
        {
            var client = CreateAuthorizedClient();
            var response = await client.DeleteAsync($"api/Reservas/CancelarReserva/{reservaId}");
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
