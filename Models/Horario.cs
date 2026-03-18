using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProyectoRestauranteC_.Models
{
    [Table("Horarios")]
    public class Horario
    {
        [Key]
        [Column("Id")]
        public int Id { get; set; }

        [Column("DiaSemana")]
        public int DiaSemana { get; set; }

        [Column("HoraApertura")]
        public TimeSpan HoraApertura { get; set; }

        [Column("HoraCierre")]
        public TimeSpan HoraCierre { get; set; }

        [Column("Activo")]
        public bool Activo { get; set; }
    }
}
