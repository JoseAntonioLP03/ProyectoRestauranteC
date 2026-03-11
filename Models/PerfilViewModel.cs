namespace ProyectoRestauranteC_.Models
{
    public class PerfilViewModel
    {
        public Usuario Usuario { get; set; } = null!;
        public List<Pedido> Pedidos { get; set; } = new();
    }
}
