using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProyectoRestauranteC_.Models
{
    [Table("Mesas")]
    public class Mesa
    {
        [Key]
        [Column("Id")]
        public int Id { get; set; }

        [Column("NumeroMesa")]
        public int NumeroMesa { get; set; }

        [Column("Capacidad")]
        public int Capacidad { get; set; }

        [Column("Activa")]
        public bool Activa { get; set; } = true;
    }
}
