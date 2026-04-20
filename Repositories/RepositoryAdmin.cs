using Microsoft.EntityFrameworkCore;
using ProyectoRestauranteC_.Data;
using ProyectoRestauranteC_.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;

namespace ProyectoRestauranteC_.Repositories
{
    public class RepositoryAdmin
    {
        private readonly RestauranteContext _context;
        private readonly IWebHostEnvironment _env;

        public RepositoryAdmin(RestauranteContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        public async Task<string?> UploadImagenAsync(IFormFile? file, string folder = "menu")
        {
            if (file == null || file.Length == 0) return null;

            var uploadsFolder = Path.Combine(_env.WebRootPath, "images", folder);
            if (!Directory.Exists(uploadsFolder))
            {
                Directory.CreateDirectory(uploadsFolder);
            }

            var uniqueFileName = Guid.NewGuid().ToString() + "_" + file.FileName;
            var filePath = Path.Combine(uploadsFolder, uniqueFileName);

            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(fileStream);
            }

            return $"/images/{folder}/{uniqueFileName}";
        }

        public async Task<object> GetStatsAsync()
        {
            return new
            {
                TotalProductos = await _context.Productos.CountAsync(),
                TotalPedidos = await _context.Pedidos.CountAsync(),
                TotalUsuarios = await _context.Usuarios.CountAsync(),
                TotalValoraciones = await _context.Valoraciones.CountAsync(),
                TotalReservas = await _context.Reservas.CountAsync(),
                TotalCategorias = await _context.Categorias.CountAsync(),
                TotalCupones = await _context.Cupones.CountAsync(),
                TotalMesas = await _context.Mesas.CountAsync(),
                PedidosPendientes = await _context.Pedidos.Where(p => p.Estado == "PendientePago").CountAsync(),
                ReservasPendientes = await _context.Reservas.Where(r => r.Estado == "Pendiente").CountAsync()
            };
        }

        // PRODUCTOS
        public async Task<List<Productos>> GetProductosAsync() => await _context.Productos.ToListAsync();
        public async Task<Productos?> GetProductoByIdAsync(int id) => await _context.Productos.FindAsync(id);
        public async Task CrearProductoAsync(Productos producto)
        {
            _context.Productos.Add(producto);
            await _context.SaveChangesAsync();
        }
        public async Task UpdateProductoAsync(Productos producto)
        {
            _context.Productos.Update(producto);
            await _context.SaveChangesAsync();
        }
        public async Task DeleteProductoAsync(Productos producto)
        {
            _context.Productos.Remove(producto);
            await _context.SaveChangesAsync();
        }

        // CATEGORIAS
        public async Task<List<Categoria>> GetCategoriasAsync() => await _context.Categorias.ToListAsync();
        public async Task<Categoria?> GetCategoriaByIdAsync(int id) => await _context.Categorias.FindAsync(id);
        public async Task CrearCategoriaAsync(Categoria categoria)
        {
            _context.Categorias.Add(categoria);
            await _context.SaveChangesAsync();
        }
        public async Task UpdateCategoriaAsync(Categoria categoria)
        {
            _context.Categorias.Update(categoria);
            await _context.SaveChangesAsync();
        }
        public async Task DeleteCategoriaAsync(Categoria categoria)
        {
            _context.Categorias.Remove(categoria);
            await _context.SaveChangesAsync();
        }

        // USUARIOS
        public async Task<List<Usuario>> GetUsuariosAsync() => await _context.Usuarios.ToListAsync();
        public async Task<Usuario?> GetUsuarioByIdAsync(int id) => await _context.Usuarios.FindAsync(id);
        public async Task UpdateUsuarioAsync(Usuario usuario)
        {
            _context.Usuarios.Update(usuario);
            await _context.SaveChangesAsync();
        }
        public async Task DeleteUsuarioAsync(Usuario usuario)
        {
            _context.Usuarios.Remove(usuario);
            await _context.SaveChangesAsync();
        }

        // PEDIDOS
        public async Task<List<Pedido>> GetPedidosAsync() => await _context.Pedidos.ToListAsync();
        public async Task<Pedido?> GetPedidoByIdAsync(int id) => await _context.Pedidos.FindAsync(id);
        public async Task UpdatePedidoAsync(Pedido pedido)
        {
            _context.Pedidos.Update(pedido);
            await _context.SaveChangesAsync();
        }
        public async Task DeletePedidoAsync(Pedido pedido, int id)
        {
            var detalles = await _context.DetallesPedido.Where(d => d.PedidoId == id).ToListAsync();
            _context.DetallesPedido.RemoveRange(detalles);
            
            _context.Pedidos.Remove(pedido);
            await _context.SaveChangesAsync();
        }
    }
}