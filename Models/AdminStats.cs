namespace ProyectoRestauranteC_.Models
{
    public class AdminStats
    {
        public int TotalProductos { get; set; }
        public int TotalPedidos { get; set; }
        public int TotalUsuarios { get; set; }
        public int TotalValoraciones { get; set; }
        public int TotalReservas { get; set; }
        public int TotalCategorias { get; set; }
        public int TotalCupones { get; set; }
        public int TotalMesas { get; set; }
        public int PedidosPendientes { get; set; }
        public int ReservasPendientes { get; set; }
    }
}
