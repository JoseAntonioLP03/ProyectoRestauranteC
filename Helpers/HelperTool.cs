using System.Security.Cryptography;

namespace ProyectoRestauranteC_.Helpers
{
    public class HelperTool
    {
        // Genera un Salt aleatorio de forma segura y lo devuelve en formato Base64
        public static string GenerateSalt()
        {
            byte[] bytes = new byte[32]; // Generamos 256 bits (32 bytes) para alta seguridad
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(bytes);
            }
            // Devolver en Base64 garantiza que sean caracteres estándar imprimibles que no se corrompan fácilmente
            return Convert.ToBase64String(bytes);
        }

        // Compara dos arrays de bytes (necesario para el login)
        public static bool CompareArrays(byte[] a, byte[] b)
        {
            if (a.Length != b.Length) return false;
            for (int i = 0; i < a.Length; i++)
            {
                if (a[i] != b[i]) return false;
            }
            return true;
        }
    }
}
