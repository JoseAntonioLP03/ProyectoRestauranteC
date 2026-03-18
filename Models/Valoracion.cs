using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProyectoRestauranteC_.Models
{
    [Table("Valoraciones")]
    public class Valoracion
    {
        [Key]
        [Column("Id")]
        public int Id { get; set; }

        [Column("UsuarioId")]
        public int UsuarioId { get; set; }

        [Column("Puntuacion")]
        public int Puntuacion { get; set; }

        [Column("Comentario")]
        public string Comentario { get; set; } = string.Empty;

        [Column("Fecha")]
        public DateTime Fecha { get; set; }

        [Column("Visible")]
        public bool Visible { get; set; }

        [ForeignKey("UsuarioId")]
        public virtual Usuario Usuario { get; set; } = null!;
    }
}

