using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProyectoRestauranteC_.Models
{
    [Table("Usuarios")]
    public class Usuario
    {
        [Key]
        [Column("Id")]
        public int Id { get; set; }

        [Column("Nombre")]
        public string Nombre { get; set; } = null!;

        [Column("Email")]
        public string Email { get; set; } = null!;

        [Column("Password")] 
        public string Password { get; set; } = null!;

        [Column("Telefono")]
        public string? Telefono { get; set; }

        [Column("Direccion")]
        public string? Direccion { get; set; }

        [Column("Rol")]
        public string Rol { get; set; } = "Cliente";

        [Column("FechaRegistro")]
        public DateTime FechaRegistro { get; set; } = DateTime.Now;

        [Column("Activo")]
        public bool Activo { get; set; } = true;

        // Propiedad de navegación (Opcional, útil para Entity Framework)
        public virtual UsuarioSeguridad? Seguridad { get; set; }
    }
}
