using Microsoft.EntityFrameworkCore;
using ProyectoRestauranteC_.Models;

namespace ProyectoRestauranteC_.Data
{
    public class RestauranteContext : DbContext
    {
        public RestauranteContext(DbContextOptions<RestauranteContext> options): base(options)
        {
        }

        
        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<UsuarioSeguridad> UsuariosSeguridad { get; set; }
        public DbSet<Categoria> Categorias { get; set; }
        public DbSet<Productos> Productos { get; set; }
        public DbSet<Cupon> Cupones { get; set; }
        public DbSet<Pedido> Pedidos { get; set; }
        public DbSet<DetallePedido> DetallesPedido { get; set; }
    }
}
