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
    }

}
