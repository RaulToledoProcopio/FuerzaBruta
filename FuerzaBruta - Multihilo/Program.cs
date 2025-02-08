using System.Security.Cryptography;
using System.Text;

class FuerzaBruta
{
    // Variable que indica si ya se ha encontrado la contraseña
    static string passwordCorrecta = null;
    static object lockObject = new object(); // Usamos un objeto para bloquear el acceso a la variable compartida

    static void Main(string[] args)
    {
        // Leemos el archivo de contraseñas y lo pasamos a una lista.
        string archivoPasswords = "passwords.txt";
        List<string> passwords = new List<string>(File.ReadAllLines(archivoPasswords));

        // Utilizamos la clase Random para generar un número aleatorio y seleccionar una contraseña aleatoria de la lista.
        Random rnd = new Random();
        string passwordElegida = passwords[rnd.Next(passwords.Count)];

        // Hasheamos la contraseña elegida con el SHA-256
        string hashpasswordElegida = GetSha256Hash(passwordElegida);

        // Mostramos la contraseña elegida y su hash en la consola.
        Console.WriteLine($"Contraseña elegida: {passwordElegida}");
        Console.WriteLine($"Hash de la contraseña elegida: {hashpasswordElegida}\n");

        // Pausa hasta que el usuario presione Enter para continuar
        Console.WriteLine("Presiona Enter para empezar a comparar las contraseñas...");
        Console.ReadLine();

        // Dividimos la lista de contraseñas en dos mitades
        int mitad = passwords.Count / 2;

        // Creamos los hilos para que cada uno busque en su parte de la lista
        Thread hilo1 = new Thread(() => BuscarPassword(passwords.GetRange(0, mitad), hashpasswordElegida, "Hilo 1"));
        Thread hilo2 = new Thread(() => BuscarPassword(passwords.GetRange(mitad, passwords.Count - mitad), hashpasswordElegida, "Hilo 2"));

        // Iniciamos los hilos
        hilo1.Start();
        hilo2.Start();

        // Esperamos a que ambos hilos terminen
        hilo1.Join();
        hilo2.Join();

        // Si se encuentra la contraseña, mostramos el resultado
        if (passwordCorrecta != null)
        {
            Console.WriteLine($"Contraseña encontrada: {passwordCorrecta}");
        }
        else
        {
            Console.WriteLine("La contraseña no fue encontrada."); // Esto es por si usamos una contraseña que no estuviera en el archivo
        }
    }

    // Función que cada hilo va a ejecutar
    static void BuscarPassword(List<string> passwords, string hashpasswordElegida, string hiloId)
    {
        foreach (string password in passwords)
        {
            // Verificamos si la contraseña ya fue encontrada
            lock (lockObject)
            {
                if (passwordCorrecta != null)
                {
                    return;
                }
            }

            // Hasheamos la contraseña que se está probando
            string hashIntento = GetSha256Hash(password);

            // Sincronizamos el acceso a la consola para que los hilos no interfieran entre sí
            lock (lockObject)
            {
                // Mostramos la contraseña que estamos probando y su hash correspondiente, con el ID del hilo
                Console.WriteLine($"{hiloId} - Probando contraseña: {password}");
                Console.WriteLine($"{hiloId} - Hash de la contraseña probada: {hashIntento}");

                // Comparamos los hashes
                if (hashIntento == hashpasswordElegida)
                {
                    // Si encontramos la contraseña correcta, la guardamos en la variable compartida
                    passwordCorrecta = password;
                    break;
                }
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
