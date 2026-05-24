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
    }

}
