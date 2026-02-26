using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProyectoRestauranteC_.Models
{
    [Table("Productos")]
    public class Productos
    {
        [Key]
        [Column("Id")]
        public int Id { get; set; }

        [Column("Nombre")]
        public string Nombre { get; set; } = null!;

        [Column("Descripcion")]
        public string? Descripcion { get; set; }

        [Column("Precio", TypeName = "decimal(10,2)")]
        public decimal Precio { get; set; }

        [Column("ImagenUrl")]
        public string? ImagenUrl { get; set; }

        [Column("CategoriaId")]
        public int CategoriaId { get; set; }

        [Column("Disponible")]
        public bool Disponible { get; set; } = true;

        [Column("FechaCreacion")]
        public DateTime FechaCreacion { get; set; } = DateTime.Now;

        // Propiedad de navegación: Un producto pertenece a una sola categoría
        [ForeignKey("CategoriaId")]
        public virtual Categoria? Categoria { get; set; }
    }
}
