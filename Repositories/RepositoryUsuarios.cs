using Microsoft.AspNetCore.Http;
using ProyectoRestauranteC_.Models;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json.Serialization;

namespace ProyectoRestauranteC_.Repositories
{
    public class RepositoryUsuarios
    {
        private readonly IHttpClientFactory httpClientFactory;
        private readonly IHttpContextAccessor httpContextAccessor;

        public RepositoryUsuarios(IHttpClientFactory httpClientFactory, IHttpContextAccessor httpContextAccessor)
        {
            this.httpClientFactory = httpClientFactory;
            this.httpContextAccessor = httpContextAccessor;
        }

        public async Task RegisterUsuarioAsync(string nombre, string email, string password, string telefono, string direccion)
        {
            var client = this.httpClientFactory.CreateClient("ApiTopMeal");
            var response = await client.PostAsJsonAsync("api/Usuarios/Register", new
            {
                Nombre = nombre,
                Email = email,
                Password = password,
                Telefono = telefono,
                Direccion = direccion
            });

            response.EnsureSuccessStatusCode();
        }

        public async Task<Usuario?> GetUsuarioByIdAsync(int id)
        {
            var client = CreateAuthorizedClient();
            var response = await client.GetAsync("api/Usuarios/Usuario");
            if (!response.IsSuccessStatusCode)
            {
                return null;
            }

            return await response.Content.ReadFromJsonAsync<Usuario>();
        }

        public async Task<Cupon?> ValidarCuponAsync(string codigo)
        {
            var client = this.httpClientFactory.CreateClient("ApiTopMeal");
            var response = await client.GetAsync($"api/Usuarios/ValidarCupon/{codigo}");
            if (!response.IsSuccessStatusCode)
            {
                return null;
            }

            return await response.Content.ReadFromJsonAsync<Cupon>();
        }

        public async Task<List<Cupon>> GetCuponesDisponiblesAsync()
        {
            var client = this.httpClientFactory.CreateClient("ApiTopMeal");
            return await client.GetFromJsonAsync<List<Cupon>>("api/Usuarios/CuponesDisponibles")
                   ?? new List<Cupon>();
        }

        public async Task<Pedido> CrearPedidoAsync(int usuarioId, List<ItemCarrito> items,
            decimal subtotal, decimal descuento, decimal total, string? direccionEntrega, int? cuponId, string tipoPedido = "Domicilio")
        {
            var client = CreateAuthorizedClient();
            var response = await client.PostAsJsonAsync("api/Usuarios/CrearPedido", new
            {
                Items = items,
                Subtotal = subtotal,
                Descuento = descuento,
                Total = total,
                DireccionEntrega = direccionEntrega,
                CuponId = cuponId,
                TipoPedido = tipoPedido
            });

            response.EnsureSuccessStatusCode();
            return (await response.Content.ReadFromJsonAsync<Pedido>())!;
        }

        public async Task<Pedido?> GetPedidoConDetallesAsync(int pedidoId, int usuarioId)
        {
            var client = CreateAuthorizedClient();
            var response = await client.GetAsync($"api/Usuarios/Pedido/{pedidoId}");
            if (!response.IsSuccessStatusCode)
            {
                return null;
            }

            return await response.Content.ReadFromJsonAsync<Pedido>();
        }

        public async Task<List<Pedido>> GetPedidosByUsuarioAsync(int usuarioId)
        {
            var client = CreateAuthorizedClient();
            var response = await client.GetAsync("api/Usuarios/PedidosUsuario");
            if (!response.IsSuccessStatusCode)
            {
                return new List<Pedido>();
            }

            return await response.Content.ReadFromJsonAsync<List<Pedido>>() ?? new List<Pedido>();
        }

        public async Task<Usuario?> ExisteUsuarioAsync(string email, string password)
        {
            var result = await LoginAsync(email, password);
            return result.Usuario;
        }

        public async Task<(Usuario? Usuario, string? Token)> LoginAsync(string email, string password)
        {
            var client = this.httpClientFactory.CreateClient("ApiTopMeal");
            var response = await client.PostAsJsonAsync("api/Auth/Login", new
            {
                Email = email,
                Password = password
            });

            if (!response.IsSuccessStatusCode)
            {
                return (null, null);
            }

            var payload = await response.Content.ReadFromJsonAsync<LoginResponse>();
            if (payload?.Response == null)
            {
                return (null, null);
            }

            var usuarioClient = CreateAuthorizedClient(payload.Response);
            var usuarioResponse = await usuarioClient.GetAsync("api/Usuarios/Usuario");
            if (!usuarioResponse.IsSuccessStatusCode)
            {
                return (null, null);
            }

            var usuario = await usuarioResponse.Content.ReadFromJsonAsync<Usuario>();
            return (usuario, payload.Response);
        }

        private HttpClient CreateAuthorizedClient(string? token = null)
        {
            var client = this.httpClientFactory.CreateClient("ApiTopMeal");
            var tokenValue = token ?? this.httpContextAccessor.HttpContext?.Session.GetString("API_TOKEN");
            if (!string.IsNullOrWhiteSpace(tokenValue))
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", tokenValue);
            }

            return client;
        }

        private class LoginResponse
        {
            [JsonPropertyName("response")]
            public string? Response { get; set; }
        }
    }
}