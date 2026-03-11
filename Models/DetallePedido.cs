using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProyectoRestauranteC_.Models
{
    [Table("DetallePedido")]
    public class DetallePedido
    {
        [Key]
        [Column("Id")]
        public int Id { get; set; }

        [Column("PedidoId")]
        public int PedidoId { get; set; }

        [Column("ProductoId")]
        public int ProductoId { get; set; }

        [Column("Cantidad")]
        public int Cantidad { get; set; }

        [Column("PrecioUnitario", TypeName = "decimal(10,2)")]
        public decimal PrecioUnitario { get; set; }

        [ForeignKey("ProductoId")]
        public virtual Productos? Producto { get; set; }
    }
}
