using System;
using System.IO;
using System.Diagnostics;

namespace SistemadeGestióndeBiblioteca
{
    class Program
    {
        struct Libro
        {
            public string codigo;
            public string titulo;
            public string autor;
            public string editorial;
            public int añoDePublicacion;
            public int cantidadDeEjemplaresDisponibles;
        }
        struct Usuario
        {
            public string carnet;
            public string nombreCompleto;
            public string carrera;
            public string correoElectronico;
            public int telefono;
            public bool estado;
        }
        struct Prestamo
        {
            public string carneUsuario;
            public string codigoLibro;
            public string fechaPrestamo;
            public string fechaDevolucionEstimada;
            public string estado;
        }

        static Libro[] libros = new Libro[10];
        static Usuario[] usuarios = new Usuario[5];
        static Prestamo[] prestamos = new Prestamo[10];
        static int cantLibros = 0;
        static int cantUsuarios = 0;
        static int cantPrestamos = 0;

        static string LeerTexto(string etiqueta)
        {
            string valor;
            do
            {
                Console.Write(etiqueta);
                valor = Console.ReadLine();
                if (string.IsNullOrWhiteSpace(valor))
                    Console.WriteLine("El campo no puede estar vacío.");
            } while (string.IsNullOrWhiteSpace(valor));
            return valor;
        }

        static int LeerEntero(string etiqueta)
        {
            while (true)
            {
                Console.Write(etiqueta);
                try
                {
                    return int.Parse(Console.ReadLine());
                }
                catch
                {
                    Console.WriteLine("Ingrese un número entero válido.");
                }
            }
        }
        static string LeerCodigoLibro(string etiqueta)
        {
            while (true)
            {
                Console.Write(etiqueta);
                string v = Console.ReadLine();
                if (string.IsNullOrWhiteSpace(v))
                {
                    Console.WriteLine("El campo no puede estar vacío.");
                    continue;
                }
                if (v.Length != 8)
                {
                    Console.WriteLine("El código debe tener exactamente 8 caracteres (ej. LIB00001).");
                    continue;
                }
                bool ok = true;
                foreach (char c in v) if (!char.IsLetterOrDigit(c))
                    {
                        ok = false;
                        break;
                    }
                if (!ok)
                {
                    Console.WriteLine("Solo se permiten letras y dígitos.");
                    continue;
                }
                return v;
            }
        }

        static string LeerCarnet(string etiqueta)
        {
            while (true)
            {
                Console.Write(etiqueta);
                string v = Console.ReadLine();
                if (string.IsNullOrWhiteSpace(v))
                {
                    Console.WriteLine("El campo no puede estar vacío.");
                    continue;
                }
                if (v.Length != 8)
                {
                    Console.WriteLine("El carné debe tener exactamente 8 dígitos.");
                    continue;
                }
                bool ok = true;
                foreach (char c in v)
                    if (!char.IsDigit(c))
                    {
                        ok = false;
                        break;
                    }
                if (!ok)
                {
                    Console.WriteLine("El carné solo debe contener dígitos.");
                    continue;
                }
                return v;
            }
        }

        static string LeerCorreo(string etiqueta)
        {
            while (true)
            {
                Console.Write(etiqueta);
                string v = Console.ReadLine();
                if (string.IsNullOrWhiteSpace(v))
                {
                    Console.WriteLine("El campo no puede estar vacío.");
                    continue;
                }
                int at = v.IndexOf('@');
                if (at < 1)
                {
                    Console.WriteLine("El correo debe contener '@'.");
                    continue;
                }
                if (v.IndexOf('.', at) <= at + 1)
                {
                    Console.WriteLine("El correo debe tener un '.' después del '@'.");
                    continue;
                }
                return v;
            }
        }

        static int LeerAño(string etiqueta)
        {
            while (true)
            {
                int año = LeerEntero(etiqueta);
                if (año >= 1900 && año <= DateTime.Now.Year)
                    return año;
                Console.WriteLine($"El año debe estar entre 1900 y {DateTime.Now.Year}.");
            }
        }

        static int LeerEjemplares(string etiqueta)
        {
            while (true)
            {
                int v = LeerEntero(etiqueta);
                if (v >= 0)
                    return v;
                Console.WriteLine("La cantidad no puede ser negativa.");
            }
        }

        static string LeerFecha(string etiqueta)
        {
            while (true)
            {
                Console.Write(etiqueta);
                string v = Console.ReadLine();
                if (!string.IsNullOrWhiteSpace(v) && v.Length == 10 && v[2] == '/' && v[5] == '/')
                    return v;
                Console.WriteLine("Formato inválido. Use dd/mm/yyyy.");
            }
        }
        static readonly string dataArchivo = "Data/";

        static void CargarDatos()
        {
            if (!Directory.Exists(dataArchivo)) Directory.CreateDirectory(dataArchivo);

            string rutaLibros = dataArchivo + "libros.csv";
            if (File.Exists(rutaLibros))
            {
                foreach (string linea in File.ReadAllLines(rutaLibros))
                {
                    if (string.IsNullOrWhiteSpace(linea) || cantLibros >= libros.Length)
                        continue;
                    string[] p = linea.Split(';');
                    if (p.Length < 6)
                        continue;
                    libros[cantLibros].codigo = p[0];
                    libros[cantLibros].titulo = p[1];
                    libros[cantLibros].autor = p[2];
                    libros[cantLibros].editorial = p[3];
                    if (int.TryParse(p[4], out int año))
                        libros[cantLibros].añoDePublicacion = año;
                    if (int.TryParse(p[5], out int ej))
                        libros[cantLibros].cantidadDeEjemplaresDisponibles = ej;
                    cantLibros++;
                }
            }

            string rutaUsuarios = dataArchivo + "usuarios.txt";
            if (File.Exists(rutaUsuarios))
            {
                foreach (string linea in File.ReadAllLines(rutaUsuarios))
                {
                    if (string.IsNullOrWhiteSpace(linea) || cantUsuarios >= usuarios.Length)
                        continue;
                    string[] p = linea.Split(';');
                    if (p.Length < 6)
                        continue;
                    usuarios[cantUsuarios].carnet = p[0];
                    usuarios[cantUsuarios].nombreCompleto = p[1];
                    usuarios[cantUsuarios].carrera = p[2];
                    usuarios[cantUsuarios].correoElectronico = p[3];
                    if (int.TryParse(p[4], out int tel))
                        usuarios[cantUsuarios].telefono = tel;
                    usuarios[cantUsuarios].estado = p[5].Trim() == "1";
                    cantUsuarios++;
                }
            }

            string rutaPrestamos = dataArchivo + "prestamos.txt";
            if (File.Exists(rutaPrestamos))
            {
                foreach (string linea in File.ReadAllLines(rutaPrestamos))
                {
                    if (string.IsNullOrWhiteSpace(linea) || cantPrestamos >= prestamos.Length)
                        continue;
                    string[] p = linea.Split(';');
                    if (p.Length < 5)
                        continue;
                    prestamos[cantPrestamos].carneUsuario = p[0];
                    prestamos[cantPrestamos].codigoLibro = p[1];
                    prestamos[cantPrestamos].fechaPrestamo = p[2];
                    prestamos[cantPrestamos].fechaDevolucionEstimada = p[3];
                    prestamos[cantPrestamos].estado = p[4].Trim();
                    cantPrestamos++;
                }
            }
        }
    }

}
