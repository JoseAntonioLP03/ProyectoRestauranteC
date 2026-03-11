using Microsoft.EntityFrameworkCore;
using ProyectoRestauranteC_.Data;
using ProyectoRestauranteC_.Helpers;
using ProyectoRestauranteC_.Models;

namespace ProyectoRestauranteC_.Repositories
{
    public class RepositoryUsuarios
    {
        private RestauranteContext context;

        public RepositoryUsuarios(RestauranteContext context)
        {
            this.context = context;
        }

        public async Task RegisterUsuarioAsync(string nombre, string email, string password, string telefono, string direccion)
        {
            //Creamos el objeto Usuario 
            Usuario user = new Usuario();
            user.Nombre = nombre;
            user.Email = email;
            user.Password = password;
            user.Telefono = telefono;
            user.Direccion = direccion;
            user.Rol = "Cliente";
            user.FechaRegistro = DateTime.Now;
            user.Activo = true;

            this.context.Usuarios.Add(user);
            await this.context.SaveChangesAsync();

            UsuarioSeguridad seguridad = new UsuarioSeguridad();
            seguridad.UsuarioId = user.Id;

            string salt = HelperTool.GenerateSalt();
            seguridad.Salt = salt;

            byte[] hashBytes = HelperEncriptar.EncryptPassword(password, salt);
            seguridad.PasswordHash = Convert.ToBase64String(hashBytes);

            this.context.UsuariosSeguridad.Add(seguridad);
            await this.context.SaveChangesAsync();
        }

        public async Task<Usuario?> GetUsuarioByIdAsync(int id)
        {
            return await this.context.Usuarios.FindAsync(id);
        }

        public async Task<Cupon?> ValidarCuponAsync(string codigo)
        {
            return await this.context.Cupones.FirstOrDefaultAsync(c =>
                c.Codigo == codigo &&
                c.Activo &&
                c.VecesUsado < c.UsoMaximo &&
                c.FechaInicio <= DateTime.Now &&
                c.FechaFin >= DateTime.Now);
        }

        public async Task<List<Cupon>> GetCuponesDisponiblesAsync()
        {
            return await this.context.Cupones
                .Where(c => c.Activo && c.VecesUsado < c.UsoMaximo &&
                            c.FechaInicio <= DateTime.Now && c.FechaFin >= DateTime.Now)
                .ToListAsync();
        }

        public async Task<Pedido> CrearPedidoAsync(int usuarioId, List<ItemCarrito> items,
            decimal subtotal, decimal descuento, decimal total, string? direccionEntrega, int? cuponId)
        {
            var pedido = new Pedido
            {
                UsuarioId = usuarioId,
                FechaPedido = DateTime.Now,
                TipoPedido = "Domicilio",
                Estado = "PendientePago",
                Subtotal = subtotal,
                Descuento = descuento,
                Total = total,
                DireccionEntrega = direccionEntrega,
                CuponId = cuponId,
                Detalles = items.Select(item => new DetallePedido
                {
                    ProductoId = item.Id,
                    Cantidad = item.Cantidad,
                    PrecioUnitario = item.Precio
                }).ToList()
            };

            this.context.Pedidos.Add(pedido);

            if (cuponId.HasValue)
            {
                var cupon = await this.context.Cupones.FindAsync(cuponId.Value);
                if (cupon != null) cupon.VecesUsado++;
            }

            await this.context.SaveChangesAsync();
            return pedido;
        }

        public async Task<Pedido?> GetPedidoConDetallesAsync(int pedidoId, int usuarioId)
        {
            return await this.context.Pedidos
                .Include(p => p.Detalles)
                    .ThenInclude(d => d.Producto)
                .FirstOrDefaultAsync(p => p.Id == pedidoId && p.UsuarioId == usuarioId);
        }

        public async Task<List<Pedido>> GetPedidosByUsuarioAsync(int usuarioId)
        {
            return await this.context.Pedidos
                .Include(p => p.Detalles)
                    .ThenInclude(d => d.Producto)
                .Where(p => p.UsuarioId == usuarioId)
                .OrderByDescending(p => p.FechaPedido)
                .ToListAsync();
        }

        public async Task<Usuario?> ExisteUsuarioAsync(string email, string password)
        {
            Usuario? user = await this.context.Usuarios
                .Include(u => u.Seguridad)
                .FirstOrDefaultAsync(u => u.Email == email);

            if (user == null || user.Seguridad == null)
            {
                return null;
            }

            string salt = user.Seguridad.Salt;
            string hashGuardado = user.Seguridad.PasswordHash;
            byte[] tempHashBytes = HelperEncriptar.EncryptPassword(password, salt);
            string hashActual = Convert.ToBase64String(tempHashBytes);
            if (hashActual == hashGuardado)
            {
                return user;
            }
            return null;
        }
    }
}