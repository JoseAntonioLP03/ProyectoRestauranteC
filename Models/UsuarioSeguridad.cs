using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProyectoRestauranteC_.Models
{
    [Table("UsuariosSeguridad")]
    public class UsuarioSeguridad
    {
        [Key]
        [Column("UsuarioId")]
        public int UsuarioId { get; set; }

        [Required]
        [Column("PasswordHash")] // Aquí guardas el hash largo
        public string PasswordHash { get; set; } = null!;

        [Required]
        [Column("Salt")] // Aquí guardas el salt único
        public string Salt { get; set; } = null!;

        // Relación con la tabla Usuarios
        [ForeignKey("UsuarioId")]
        public virtual Usuario Usuario { get; set; } = null!;
    }
}
