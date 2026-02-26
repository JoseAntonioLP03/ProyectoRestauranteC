using Microsoft.EntityFrameworkCore;
using ProyectoRestauranteC_.Data;
using ProyectoRestauranteC_.Helpers;
using ProyectoRestauranteC_.Models;

namespace ProyectoRestauranteC_.Repositories
{
    public class RepositoryUsuarios
    {
        private RestauranteContext context;

        public RepositoryUsuarios(RestauranteContext context)
        {
            this.context = context;
        }

        public async Task RegisterUsuarioAsync(string nombre, string email, string password, string telefono, string direccion)
        {
            //Creamos el objeto Usuario 
            Usuario user = new Usuario();
            user.Nombre = nombre;
            user.Email = email;
            user.Password = password;
            user.Telefono = telefono;
            user.Direccion = direccion;
            user.Rol = "Cliente";
            user.FechaRegistro = DateTime.Now;
            user.Activo = true;

            this.context.Usuarios.Add(user);
            await this.context.SaveChangesAsync();

            UsuarioSeguridad seguridad = new UsuarioSeguridad();
            seguridad.UsuarioId = user.Id;

            string salt = HelperTool.GenerateSalt();
            seguridad.Salt = salt;

            byte[] hashBytes = HelperEncriptar.EncryptPassword(password, salt);
            seguridad.PasswordHash = Convert.ToBase64String(hashBytes);

            this.context.UsuariosSeguridad.Add(seguridad);
            await this.context.SaveChangesAsync();
        }

        public async Task<Usuario?> ExisteUsuarioAsync(string email, string password)
        {
            Usuario? user = await this.context.Usuarios
                .Include(u => u.Seguridad)
                .FirstOrDefaultAsync(u => u.Email == email);

            if (user == null || user.Seguridad == null)
            {
                return null;
            }

            string salt = user.Seguridad.Salt;
            string hashGuardado = user.Seguridad.PasswordHash;
            byte[] tempHashBytes = HelperEncriptar.EncryptPassword(password, salt);
            string hashActual = Convert.ToBase64String(tempHashBytes);
            if (hashActual == hashGuardado)
            {
                return user;
            }
            return null;
        }
    }
}