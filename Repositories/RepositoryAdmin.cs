using Microsoft.AspNetCore.Http;
using ProyectoRestauranteC_.Models;
using System.Globalization;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace ProyectoRestauranteC_.Repositories
{
    public class RepositoryAdmin
    {
        private readonly IHttpClientFactory httpClientFactory;
        private readonly IHttpContextAccessor httpContextAccessor;

        public RepositoryAdmin(IHttpClientFactory httpClientFactory, IHttpContextAccessor httpContextAccessor)
        {
            this.httpClientFactory = httpClientFactory;
            this.httpContextAccessor = httpContextAccessor;
        }

        public async Task<AdminStats> GetStatsAsync()
        {
            var client = CreateAuthorizedClient();
            var response = await client.GetAsync("api/Admin/Stats");
            response.EnsureSuccessStatusCode();
            return (await response.Content.ReadFromJsonAsync<AdminStats>())!;
        }

        // PRODUCTOS
        public async Task<List<Productos>> GetProductosAsync()
        {
            var client = CreateAuthorizedClient();
            return await client.GetFromJsonAsync<List<Productos>>("api/Admin/Productos")
                   ?? new List<Productos>();
        }

        public async Task<Productos?> GetProductoByIdAsync(int id)
        {
            var client = CreateAuthorizedClient();
            var response = await client.GetAsync($"api/Admin/Producto/{id}");
            if (!response.IsSuccessStatusCode)
            {
                return null;
            }

            return await response.Content.ReadFromJsonAsync<Productos>();
        }

        public async Task CrearProductoAsync(Productos producto, IFormFile? file)
        {
            var client = CreateAuthorizedClient();
            using var content = BuildProductoFormContent(producto, file, includeId: false);
            var response = await client.PostAsync("api/Admin/CrearProducto", content);
            response.EnsureSuccessStatusCode();
        }

        public async Task UpdateProductoAsync(Productos producto, IFormFile? file)
        {
            var client = CreateAuthorizedClient();
            using var content = BuildProductoFormContent(producto, file, includeId: true);
            var response = await client.PutAsync($"api/Admin/UpdateProducto/{producto.Id}", content);
            response.EnsureSuccessStatusCode();
        }

        public async Task DeleteProductoAsync(Productos producto)
        {
            var client = CreateAuthorizedClient();
            var response = await client.DeleteAsync($"api/Admin/DeleteProducto/{producto.Id}");
            response.EnsureSuccessStatusCode();
        }

        // CATEGORIAS
        public async Task<List<Categoria>> GetCategoriasAsync()
        {
            var client = CreateAuthorizedClient();
            return await client.GetFromJsonAsync<List<Categoria>>("api/Admin/Categorias")
                   ?? new List<Categoria>();
        }

        public async Task<Categoria?> GetCategoriaByIdAsync(int id)
        {
            var client = CreateAuthorizedClient();
            var response = await client.GetAsync($"api/Admin/Categoria/{id}");
            if (!response.IsSuccessStatusCode)
            {
                return null;
            }

            return await response.Content.ReadFromJsonAsync<Categoria>();
        }

        public async Task CrearCategoriaAsync(Categoria categoria)
        {
            var client = CreateAuthorizedClient();
            var response = await client.PostAsJsonAsync("api/Admin/CrearCategoria", categoria);
            response.EnsureSuccessStatusCode();
        }

        public async Task UpdateCategoriaAsync(Categoria categoria)
        {
            var client = CreateAuthorizedClient();
            var response = await client.PutAsJsonAsync($"api/Admin/UpdateCategoria/{categoria.Id}", categoria);
            response.EnsureSuccessStatusCode();
        }

        public async Task DeleteCategoriaAsync(Categoria categoria)
        {
            var client = CreateAuthorizedClient();
            var response = await client.DeleteAsync($"api/Admin/DeleteCategoria/{categoria.Id}");
            response.EnsureSuccessStatusCode();
        }

        // USUARIOS
        public async Task<List<Usuario>> GetUsuariosAsync()
        {
            var client = CreateAuthorizedClient();
            return await client.GetFromJsonAsync<List<Usuario>>("api/Admin/Usuarios")
                   ?? new List<Usuario>();
        }

        public async Task<Usuario?> GetUsuarioByIdAsync(int id)
        {
            var client = CreateAuthorizedClient();
            var response = await client.GetAsync($"api/Admin/Usuario/{id}");
            if (!response.IsSuccessStatusCode)
            {
                return null;
            }

            return await response.Content.ReadFromJsonAsync<Usuario>();
        }

        public async Task UpdateUsuarioAsync(Usuario usuario)
        {
            var client = CreateAuthorizedClient();
            var response = await client.PutAsJsonAsync($"api/Admin/UpdateUsuario/{usuario.Id}", usuario);
            response.EnsureSuccessStatusCode();
        }

        public async Task DeleteUsuarioAsync(Usuario usuario)
        {
            var client = CreateAuthorizedClient();
            var response = await client.DeleteAsync($"api/Admin/DeleteUsuario/{usuario.Id}");
            response.EnsureSuccessStatusCode();
        }

        // PEDIDOS
        public async Task<List<Pedido>> GetPedidosAsync()
        {
            var client = CreateAuthorizedClient();
            return await client.GetFromJsonAsync<List<Pedido>>("api/Admin/Pedidos")
                   ?? new List<Pedido>();
        }

        public async Task<Pedido?> GetPedidoByIdAsync(int id)
        {
            var client = CreateAuthorizedClient();
            var response = await client.GetAsync($"api/Admin/Pedido/{id}");
            if (!response.IsSuccessStatusCode)
            {
                return null;
            }

            return await response.Content.ReadFromJsonAsync<Pedido>();
        }

        public async Task UpdatePedidoAsync(Pedido pedido)
        {
            var client = CreateAuthorizedClient();
            var response = await client.PutAsJsonAsync($"api/Admin/UpdatePedido/{pedido.Id}", pedido);
            response.EnsureSuccessStatusCode();
        }

        public async Task DeletePedidoAsync(Pedido pedido, int id)
        {
            var client = CreateAuthorizedClient();
            var response = await client.DeleteAsync($"api/Admin/DeletePedido/{id}");
            response.EnsureSuccessStatusCode();
        }

        // RESERVAS
        public async Task<List<Reserva>> GetReservasAsync()
        {
            var client = CreateAuthorizedClient();
            return await client.GetFromJsonAsync<List<Reserva>>("api/Admin/Reservas")
                   ?? new List<Reserva>();
        }

        public async Task<Reserva?> GetReservaByIdAsync(int id)
        {
            var client = CreateAuthorizedClient();
            var response = await client.GetAsync($"api/Admin/Reserva/{id}");
            if (!response.IsSuccessStatusCode)
            {
                return null;
            }

            return await response.Content.ReadFromJsonAsync<Reserva>();
        }

        public async Task UpdateReservaAsync(Reserva reserva)
        {
            var client = CreateAuthorizedClient();
            var response = await client.PutAsJsonAsync($"api/Admin/UpdateReserva/{reserva.Id}", reserva);
            response.EnsureSuccessStatusCode();
        }

        public async Task DeleteReservaAsync(Reserva reserva)
        {
            var client = CreateAuthorizedClient();
            var response = await client.DeleteAsync($"api/Admin/DeleteReserva/{reserva.Id}");
            response.EnsureSuccessStatusCode();
        }

        // VALORACIONES
        public async Task<List<Valoracion>> GetValoracionesAsync()
        {
            var client = CreateAuthorizedClient();
            return await client.GetFromJsonAsync<List<Valoracion>>("api/Admin/Valoraciones")
                   ?? new List<Valoracion>();
        }

        public async Task<Valoracion?> GetValoracionByIdAsync(int id)
        {
            var client = CreateAuthorizedClient();
            var response = await client.GetAsync($"api/Admin/Valoracion/{id}");
            if (!response.IsSuccessStatusCode)
            {
                return null;
            }

            return await response.Content.ReadFromJsonAsync<Valoracion>();
        }

        public async Task UpdateValoracionAsync(Valoracion valoracion)
        {
            var client = CreateAuthorizedClient();
            var response = await client.PutAsJsonAsync($"api/Admin/UpdateValoracion/{valoracion.Id}", valoracion);
            response.EnsureSuccessStatusCode();
        }

        public async Task DeleteValoracionAsync(Valoracion valoracion)
        {
            var client = CreateAuthorizedClient();
            var response = await client.DeleteAsync($"api/Admin/DeleteValoracion/{valoracion.Id}");
            response.EnsureSuccessStatusCode();
        }

        // CUPONES
        public async Task<List<Cupon>> GetCuponesAsync()
        {
            var client = CreateAuthorizedClient();
            return await client.GetFromJsonAsync<List<Cupon>>("api/Admin/Cupones")
                   ?? new List<Cupon>();
        }

        public async Task<Cupon?> GetCuponByIdAsync(int id)
        {
            var client = CreateAuthorizedClient();
            var response = await client.GetAsync($"api/Admin/Cupon/{id}");
            if (!response.IsSuccessStatusCode)
            {
                return null;
            }

            return await response.Content.ReadFromJsonAsync<Cupon>();
        }

        public async Task CrearCuponAsync(Cupon cupon)
        {
            var client = CreateAuthorizedClient();
            var response = await client.PostAsJsonAsync("api/Admin/CrearCupon", cupon);
            response.EnsureSuccessStatusCode();
        }

        public async Task UpdateCuponAsync(Cupon cupon)
        {
            var client = CreateAuthorizedClient();
            var response = await client.PutAsJsonAsync($"api/Admin/UpdateCupon/{cupon.Id}", cupon);
            response.EnsureSuccessStatusCode();
        }

        public async Task DeleteCuponAsync(Cupon cupon)
        {
            var client = CreateAuthorizedClient();
            var response = await client.DeleteAsync($"api/Admin/DeleteCupon/{cupon.Id}");
            response.EnsureSuccessStatusCode();
        }

        // MESAS
        public async Task<List<Mesa>> GetMesasAsync()
        {
            var client = CreateAuthorizedClient();
            return await client.GetFromJsonAsync<List<Mesa>>("api/Admin/Mesas")
                   ?? new List<Mesa>();
        }

        public async Task<Mesa?> GetMesaByIdAsync(int id)
        {
            var client = CreateAuthorizedClient();
            var response = await client.GetAsync($"api/Admin/Mesa/{id}");
            if (!response.IsSuccessStatusCode)
            {
                return null;
            }

            return await response.Content.ReadFromJsonAsync<Mesa>();
        }

        public async Task CrearMesaAsync(Mesa mesa)
        {
            var client = CreateAuthorizedClient();
            var response = await client.PostAsJsonAsync("api/Admin/CrearMesa", mesa);
            response.EnsureSuccessStatusCode();
        }

        public async Task UpdateMesaAsync(Mesa mesa)
        {
            var client = CreateAuthorizedClient();
            var response = await client.PutAsJsonAsync($"api/Admin/UpdateMesa/{mesa.Id}", mesa);
            response.EnsureSuccessStatusCode();
        }

        public async Task DeleteMesaAsync(Mesa mesa)
        {
            var client = CreateAuthorizedClient();
            var response = await client.DeleteAsync($"api/Admin/DeleteMesa/{mesa.Id}");
            response.EnsureSuccessStatusCode();
        }

        // GALERIA
        public async Task<List<Galeria>> GetGaleriaAsync()
        {
            var client = CreateAuthorizedClient();
            return await client.GetFromJsonAsync<List<Galeria>>("api/Admin/Galeria")
                   ?? new List<Galeria>();
        }

        public async Task<Galeria?> GetGaleriaByIdAsync(int id)
        {
            var client = CreateAuthorizedClient();
            var response = await client.GetAsync($"api/Admin/Galeria/{id}");
            if (!response.IsSuccessStatusCode)
            {
                return null;
            }

            return await response.Content.ReadFromJsonAsync<Galeria>();
        }

        public async Task CrearGaleriaAsync(Galeria imagen)
        {
            var client = CreateAuthorizedClient();
            var response = await client.PostAsJsonAsync("api/Admin/CrearGaleria", imagen);
            response.EnsureSuccessStatusCode();
        }

        public async Task UpdateGaleriaAsync(Galeria imagen)
        {
            var client = CreateAuthorizedClient();
            var response = await client.PutAsJsonAsync($"api/Admin/UpdateGaleria/{imagen.Id}", imagen);
            response.EnsureSuccessStatusCode();
        }

        public async Task DeleteGaleriaAsync(Galeria imagen)
        {
            var client = CreateAuthorizedClient();
            var response = await client.DeleteAsync($"api/Admin/DeleteGaleria/{imagen.Id}");
            response.EnsureSuccessStatusCode();
        }

        // HORARIOS
        public async Task<List<Horario>> GetHorariosAsync()
        {
            var client = CreateAuthorizedClient();
            return await client.GetFromJsonAsync<List<Horario>>("api/Admin/Horarios")
                   ?? new List<Horario>();
        }

        public async Task<Horario?> GetHorarioByIdAsync(int id)
        {
            var client = CreateAuthorizedClient();
            var response = await client.GetAsync($"api/Admin/Horario/{id}");
            if (!response.IsSuccessStatusCode)
            {
                return null;
            }

            return await response.Content.ReadFromJsonAsync<Horario>();
        }

        public async Task UpdateHorarioAsync(Horario horario)
        {
            var client = CreateAuthorizedClient();
            var response = await client.PutAsJsonAsync($"api/Admin/UpdateHorario/{horario.Id}", horario);
            response.EnsureSuccessStatusCode();
        }

        public async Task<string?> UploadImagenAsync(IFormFile file, string tipo)
        {
            var client = CreateAuthorizedClient();
            using var content = new MultipartFormDataContent();
            var fileContent = new StreamContent(file.OpenReadStream());
            fileContent.Headers.ContentType = new MediaTypeHeaderValue(file.ContentType);
            content.Add(fileContent, "file", file.FileName);

            var response = await client.PostAsync($"api/Admin/UploadImagen/{tipo}", content);
            if (!response.IsSuccessStatusCode)
            {
                return null;
            }

            return await response.Content.ReadFromJsonAsync<string>()
                   ?? (await response.Content.ReadAsStringAsync());
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

        private static MultipartFormDataContent BuildProductoFormContent(Productos producto, IFormFile? file, bool includeId)
        {
            var content = new MultipartFormDataContent();

            if (includeId)
            {
                content.Add(new StringContent(producto.Id.ToString(CultureInfo.InvariantCulture)), "Id");
            }

            content.Add(new StringContent(producto.Nombre ?? string.Empty), "Nombre");
            content.Add(new StringContent(producto.Descripcion ?? string.Empty), "Descripcion");
            content.Add(new StringContent(producto.Precio.ToString(CultureInfo.InvariantCulture)), "Precio");
            content.Add(new StringContent(producto.ImagenUrl ?? string.Empty), "ImagenUrl");
            content.Add(new StringContent(producto.CategoriaId.ToString(CultureInfo.InvariantCulture)), "CategoriaId");
            content.Add(new StringContent(producto.Disponible.ToString()), "Disponible");
            content.Add(new StringContent(producto.FechaCreacion.ToString("o", CultureInfo.InvariantCulture)), "FechaCreacion");

            if (file != null)
            {
                var fileContent = new StreamContent(file.OpenReadStream());
                fileContent.Headers.ContentType = new MediaTypeHeaderValue(file.ContentType);
                content.Add(fileContent, "file", file.FileName);
            }

            return content;
        }
    }
}