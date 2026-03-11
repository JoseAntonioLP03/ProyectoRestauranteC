using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProyectoRestauranteC_.Models
{
    [Table("Cupones")]
    public class Cupon
    {
        [Key]
        [Column("Id")]
        public int Id { get; set; }

        [Column("Codigo")]
        public string Codigo { get; set; } = null!;

        [Column("TipoDescuento")]
        public string TipoDescuento { get; set; } = null!;

        [Column("ValorDescuento", TypeName = "decimal(10,2)")]
        public decimal ValorDescuento { get; set; }

        [Column("FechaInicio")]
        public DateTime FechaInicio { get; set; }

        [Column("FechaFin")]
        public DateTime FechaFin { get; set; }

        [Column("UsoMaximo")]
        public int UsoMaximo { get; set; }

        [Column("VecesUsado")]
        public int VecesUsado { get; set; }

        [Column("Activo")]
        public bool Activo { get; set; }
    }
}
