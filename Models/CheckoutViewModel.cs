namespace ProyectoRestauranteC_.Models
{
    public class CheckoutViewModel
    {
        public Usuario Usuario { get; set; } = null!;
        public List<ItemCarrito> Items { get; set; } = new();
        public Cupon? CuponAplicado { get; set; }
        public decimal Descuento { get; set; }
        public decimal Subtotal => Items.Sum(i => i.Subtotal);
        public decimal Envio => 3.00m;
        public decimal Tarifas => 2.05m;
        public decimal Total => Math.Max(0, Subtotal + Envio + Tarifas - Descuento);
    }
}
