using System.Security.Cryptography;
using System.Text;
class FuerzaBruta
{
    static void Main(string[] args)
    {
        // Leemos el archivo de contraseñas y lo pasamos a una lista.
        string archivoPasswords = "passwords.txt";
        List<string> passwords = new List<string>(File.ReadAllLines(archivoPasswords));
        
        // Utilizamos la clase Random para generar un número aleatorio y seleccionar una contraseña aleatoria de la lista.
        Random rnd = new Random();
        string passwordElegida = passwords[rnd.Next(passwords.Count)];
        
        // Hasheamos la contraseña elegida con el SHA-256 porque es el que me ha recomendado un amigo.
        string hashpasswordElegida = GetSha256Hash(passwordElegida);

        
        // Mostramos la contraseña elegida y su hash en la consola.
        Console.WriteLine($"Contraseña elegida: {passwordElegida}");
        Console.WriteLine($"Hash de la contraseña elegida: {hashpasswordElegida}\n");
        
        // Pausa hasta que el usuario presione Enter para continuar
        Console.WriteLine("Presiona Enter para empezar a comparar las contraseñas...");
        Console.ReadLine();  // Esto hace que el programa espere a que el usuario presione Enter
        
        // Recorremos todas las contraseñas en el archivo, las hasheamos y las comparamos una por una.
        foreach (string password in passwords)
        {
            string hashIntento = GetSha256Hash(password);
            
            // Mostramos la contraseña que estamos probando y su hash correspondiente.
            Console.WriteLine($"Probando contraseña: {password}");
            Console.WriteLine($"Hash de la contraseña probada: {hashIntento}");

            // Comparamos los hashes de la contraseña probada y la contraseña elegida.
            if (hashIntento == hashpasswordElegida)
            {
                // Si encontramos la contraseña correcta, mostramos el resultado y salimos del bucle.
                Console.WriteLine($"Contraseña encontrada: {password}\n");
                break;
            }
        }
    }

    // Obtenemos el hash SHA256 de una contraseña.
    static string GetSha256Hash(string input)
    {
        using (SHA256 sha256Hash = SHA256.Create())
        {
            byte[] data = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(input));
            StringBuilder sBuilder = new StringBuilder();
            foreach (byte byteValue in data)
            {
                sBuilder.Append(byteValue.ToString("x2"));
            }
            return sBuilder.ToString();
        }
    }
}