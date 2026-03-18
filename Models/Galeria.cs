using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProyectoRestauranteC_.Models
{
    [Table("ImagenesGaleria")]
    public class Galeria
    {
        [Key]
        [Column("Id")]
        public int Id { get; set; }

        [Column("UrlImagen")]
        public string UrlImagen { get; set; } = string.Empty;

        [Column("Descripcion")]
        public string Descripcion { get; set; } = string.Empty;

        [Column("Tipo")]
        public string Tipo { get; set; } = string.Empty;

        [Column("Activa")]
        public bool Activa { get; set; }
    }
}
