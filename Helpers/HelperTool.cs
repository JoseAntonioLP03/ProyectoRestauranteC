namespace ProyectoRestauranteC_.Helpers
{
    public class HelperTool
    {
        // Genera un Salt aleatorio y lo devuelve como string
        public static string GenerateSalt()
        {
            Random random = new Random();
            string salt = "";
            for (int i = 1; i <= 50; i++)
            {
                int aleatorio = random.Next(1, 255);
                char letra = Convert.ToChar(aleatorio);
                salt += letra;
            }
            return salt;
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
