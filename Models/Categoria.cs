using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProyectoRestauranteC_.Models
{
    [Table("Categorias")]
    public class Categoria
    {
        [Key]
        [Column("Id")]
        public int Id { get; set; }

        [Column("Nombre")]
        public string Nombre { get; set; } = null!;

        [Column("Descripcion")]
        public string? Descripcion { get; set; }

        [Column("Activo")]
        public bool Activo { get; set; } = true;

        // Propiedad de navegación: Una categoría contiene muchos productos
        public virtual ICollection<Productos> Productos { get; set; } = new List<Productos>();
    }
}
