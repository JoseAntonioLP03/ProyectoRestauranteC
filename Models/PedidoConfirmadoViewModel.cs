namespace ProyectoRestauranteC_.Models
{
    public class PedidoConfirmadoViewModel
    {
        public Pedido Pedido { get; set; } = null!;
        public Usuario Usuario { get; set; } = null!;
        public List<DetallePedido> Detalles { get; set; } = new();
    }
}
