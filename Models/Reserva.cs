using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProyectoRestauranteC_.Models
{
    [Table("Reservas")]
    public class Reserva
    {
        [Key]
        [Column("Id")]
        public int Id { get; set; }

        [Column("UsuarioId")]
        public int UsuarioId { get; set; }

        [Column("MesaId")]
        public int MesaId { get; set; }

        [Column("FechaReserva")]
        public DateTime FechaReserva { get; set; }

        [Column("NumeroPersonas")]
        public int NumeroPersonas { get; set; }

        [Column("Estado")]
        public string Estado { get; set; } = "Pendiente";

        [Column("FechaCreacion")]
        public DateTime FechaCreacion { get; set; } = DateTime.Now;

        [Column("FechaConfirmacion")]
        public DateTime? FechaConfirmacion { get; set; }

        [ForeignKey("UsuarioId")]
        public virtual Usuario? Usuario { get; set; }

        [ForeignKey("MesaId")]
        public virtual Mesa? Mesa { get; set; }
    }
}
