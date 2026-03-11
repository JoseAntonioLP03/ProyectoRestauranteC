using System.Security.Cryptography;
using System.Text;

namespace ProyectoRestauranteC_.Helpers
{
    public class HelperEncriptar
    {
        // Encripta la contraseña usando el Salt
        public static byte[] EncryptPassword(string password, string salt)
        {
            string contenido = password + salt;
            SHA512 managed = SHA512.Create();
            byte[] salida = Encoding.UTF8.GetBytes(contenido);

            // 15 iteraciones como pide el profesor
            for (int i = 1; i <= 15; i++)
            {
                salida = managed.ComputeHash(salida);
            }
            managed.Clear();
            return salida;
        }
    }
}
