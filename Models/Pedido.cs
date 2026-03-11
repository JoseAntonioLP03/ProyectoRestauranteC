using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProyectoRestauranteC_.Models
{
    [Table("Pedidos")]
    public class Pedido
    {
        [Key]
        [Column("Id")]
        public int Id { get; set; }

        [Column("UsuarioId")]
        public int UsuarioId { get; set; }

        [Column("FechaPedido")]
        public DateTime FechaPedido { get; set; }

        [Column("TipoPedido")]
        public string TipoPedido { get; set; } = null!;

        [Column("Estado")]
        public string Estado { get; set; } = null!;

        [Column("Subtotal", TypeName = "decimal(10,2)")]
        public decimal Subtotal { get; set; }

        [Column("Descuento", TypeName = "decimal(10,2)")]
        public decimal Descuento { get; set; }

        [Column("Total", TypeName = "decimal(10,2)")]
        public decimal Total { get; set; }

        [Column("DireccionEntrega")]
        public string? DireccionEntrega { get; set; }

        [Column("CuponId")]
        public int? CuponId { get; set; }

        public virtual ICollection<DetallePedido> Detalles { get; set; } = new List<DetallePedido>();
    }
}
