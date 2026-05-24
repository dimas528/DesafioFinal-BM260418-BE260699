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
        static void GuardarDatos()
        {
            if (!Directory.Exists(dataArchivo)) Directory.CreateDirectory(dataArchivo);

            using (StreamWriter sw = new StreamWriter(dataArchivo + "libros.csv", false))
                for (int i = 0; i < cantLibros; i++)
                    sw.WriteLine($"{libros[i].codigo};{libros[i].titulo};{libros[i].autor};{libros[i].editorial};{libros[i].añoDePublicacion};{libros[i].cantidadDeEjemplaresDisponibles}");

            using (StreamWriter sw = new StreamWriter(dataArchivo + "usuarios.txt", false))
                for (int i = 0; i < cantUsuarios; i++)
                    sw.WriteLine($"{usuarios[i].carnet};{usuarios[i].nombreCompleto};{usuarios[i].carrera};{usuarios[i].correoElectronico};{usuarios[i].telefono};{(usuarios[i].estado ? "1" : "0")}");

            using (StreamWriter sw = new StreamWriter(dataArchivo + "prestamos.txt", false))
                for (int i = 0; i < cantPrestamos; i++)
                    sw.WriteLine($"{prestamos[i].carneUsuario};{prestamos[i].codigoLibro};{prestamos[i].fechaPrestamo};{prestamos[i].fechaDevolucionEstimada};{prestamos[i].estado}");

            Console.WriteLine("Datos guardados correctamente.");
        }

        static void AbrirArchivo(string ruta)
        {
            try
            {
                string rutaCompleta = Path.GetFullPath(ruta);
                if (File.Exists(rutaCompleta))
                    Process.Start(new ProcessStartInfo(rutaCompleta) { UseShellExecute = true });
                else
                    Console.WriteLine("Archivo no encontrado: " + rutaCompleta);
            }
            catch (Exception ex) { Console.WriteLine("Error al abrir archivo: " + ex.Message); }
        }
        static void Main(string[] args)
        {
            CargarDatos();

            int Opcion = 0;
            do
            {
                Console.Clear();
                Console.WriteLine("Biblioteca Universitaria");
                Console.WriteLine("1. Gestión de Libros");
                Console.WriteLine("2. Gestión de Usuarios");
                Console.WriteLine("3. Gestión de Préstamos");
                Console.WriteLine("4. Salir del Sistema");
                Console.Write("Seleccione una opción: ");
                Opcion = int.Parse(Console.ReadLine());
                switch (Opcion)
                {
                    case 1: GestióndeLibros(); break;
                    case 2: GestióndeUsuarios(); break;
                    case 3: GestióndePréstamos(); break;
                    case 4:
                        GuardarDatos();
                        break;
                    default:
                        Console.WriteLine("Opción no válida. Presione una tecla para continuar");
                        Console.ReadKey();
                        break;
                }
            } while (Opcion != 4);
        }
        static void GestióndeLibros()
        {
            int opcion = 0;
            do
            {
                Console.Clear();
                Console.WriteLine("Gestión de Libros");
                Console.WriteLine("1. Registro de nuevos libros");
                Console.WriteLine("2. Búsqueda de libros por código");
                Console.WriteLine("3. Listado completo de libros registrados");
                Console.WriteLine("4. Eliminación de un libro");
                Console.WriteLine("5. Volver al menú principal");
                Console.Write("Seleccione una opción: ");
                opcion = int.Parse(Console.ReadLine());
                switch (opcion)
                {
                    case 1:
                        Console.Clear();
                        Console.WriteLine("Registro de Nuevo Libro");
                        if (cantLibros >= libros.Length)
                        {
                            Console.WriteLine("No hay espacio para más libros.");
                            Console.ReadKey();
                            break;
                        }
                        libros[cantLibros].codigo = LeerCodigoLibro("Código (8 caracteres alfanuméricos): ");
                        libros[cantLibros].titulo = LeerTexto("Título: ");
                        libros[cantLibros].autor = LeerTexto("Autor: ");
                        libros[cantLibros].editorial = LeerTexto("Editorial: ");
                        libros[cantLibros].añoDePublicacion = LeerAño("Año de publicación: ");
                        libros[cantLibros].cantidadDeEjemplaresDisponibles = LeerEjemplares("Cantidad de ejemplares disponibles: ");
                        cantLibros++;
                        GuardarDatos();
                        Console.WriteLine("Libro registrado exitosamente.");
                        AbrirArchivo(dataArchivo + "libros.csv");
                        Console.ReadKey();
                        break;

                    case 2:
                        Console.Clear();
                        Console.WriteLine("Búsqueda de Libro por Código");
                        Console.Write("Ingrese el código del libro: ");
                        string codigoBuscar = Console.ReadLine();
                        bool encontrado = false;
                        for (int i = 0; i < cantLibros; i++)
                        {
                            if (libros[i].codigo == codigoBuscar)
                            {
                                Console.WriteLine("Código:       " + libros[i].codigo);
                                Console.WriteLine("Título:       " + libros[i].titulo);
                                Console.WriteLine("Autor:        " + libros[i].autor);
                                Console.WriteLine("Editorial:    " + libros[i].editorial);
                                Console.WriteLine("Año:          " + libros[i].añoDePublicacion);
                                Console.WriteLine("Ejemplares:   " + libros[i].cantidadDeEjemplaresDisponibles);
                                encontrado = true;
                                break;
                            }
                        }
                        if (!encontrado)
                            Console.WriteLine("Libro no encontrado.");
                        Console.ReadKey();
                        break;
                    case 3:
                        Console.Clear();
                        Console.WriteLine("Listado Completo de Libros");
                        if (cantLibros == 0)
                        {
                            Console.WriteLine("No hay libros registrados.");
                        }
                        else
                        {
                            for (int i = 0; i < cantLibros; i++)
                            {
                                Console.WriteLine("Código:       " + libros[i].codigo);
                                Console.WriteLine("Título:       " + libros[i].titulo);
                                Console.WriteLine("Autor:        " + libros[i].autor);
                                Console.WriteLine("Editorial:    " + libros[i].editorial);
                                Console.WriteLine("Año:          " + libros[i].añoDePublicacion);
                                Console.WriteLine("Ejemplares:   " + libros[i].cantidadDeEjemplaresDisponibles);
                            }
                            Console.WriteLine("Total de libros: " + cantLibros);
                        }
                        Console.ReadKey();
                        break;

                    case 4:
                        Console.Clear();
                        Console.WriteLine("Eliminación de Libro");
                        Console.Write("Ingrese el código del libro a eliminar: ");
                        string codigoEliminar = Console.ReadLine();
                        int posEliminar = -1;
                        for (int i = 0; i < cantLibros; i++)
                        {
                            if (libros[i].codigo == codigoEliminar)
                            {
                                posEliminar = i;
                                break;
                            }
                        }
                        if (posEliminar == -1)
                        {
                            Console.WriteLine("Libro no encontrado.");
                        }
                        else
                        {
                            for (int i = posEliminar; i < cantLibros - 1; i++)
                                libros[i] = libros[i + 1];
                            cantLibros--;
                            GuardarDatos();
                            Console.WriteLine("Libro eliminado exitosamente.");
                            AbrirArchivo(dataArchivo + "libros.csv");
                        }
                        Console.ReadKey();
                        break;
                    case 5:
                        break;
                    default:
                        Console.WriteLine("Opción no válida. Presione una tecla para continuar");
                        Console.ReadKey();
                        break;
                }
            } while (opcion != 5);
        }
        static void GestióndeUsuarios()
        {
            int opcion = 0;
            do
            {
                Console.Clear();
                Console.WriteLine("Gestión de Usuarios");
                Console.WriteLine("1. Registro de nuevo usuario");
                Console.WriteLine("2. Búsqueda de usuario por carnet");
                Console.WriteLine("3. Listado completo de usuarios");
                Console.WriteLine("4. Volver al menú principal");
                Console.Write("Seleccione una opción: ");
                opcion = int.Parse(Console.ReadLine());
                switch (opcion)
                {
                    case 1:
                        Console.Clear();
                        Console.WriteLine("Registro de Nuevo Usuario");
                        if (cantUsuarios >= usuarios.Length)
                        {
                            Console.WriteLine("No hay espacio para más usuarios.");
                            Console.ReadKey();
                            break;
                        }
                        usuarios[cantUsuarios].carnet = LeerCarnet("Carnet (8 dígitos): ");
                        usuarios[cantUsuarios].nombreCompleto = LeerTexto("Nombre completo: ");
                        usuarios[cantUsuarios].carrera = LeerTexto("Carrera: ");
                        usuarios[cantUsuarios].correoElectronico = LeerCorreo("Correo electrónico: ");
                        usuarios[cantUsuarios].telefono = LeerEntero("Teléfono: ");
                        usuarios[cantUsuarios].estado = true;
                        cantUsuarios++;
                        GuardarDatos();
                        Console.WriteLine("Usuario registrado exitosamente.");
                        AbrirArchivo(dataArchivo + "usuarios.txt");
                        Console.ReadKey();
                        break;

                    case 2:
                        Console.Clear();
                        Console.WriteLine("Búsqueda de Usuario por Carnet");
                        Console.Write("Ingrese el carnet: ");
                        string carnetBuscar = Console.ReadLine();
                        bool encontradoU = false;
                        for (int i = 0; i < cantUsuarios; i++)
                        {
                            if (usuarios[i].carnet == carnetBuscar)
                            {
                                Console.WriteLine("----------------------------------------");
                                Console.WriteLine("Carnet:   " + usuarios[i].carnet);
                                Console.WriteLine("Nombre:   " + usuarios[i].nombreCompleto);
                                Console.WriteLine("Carrera:  " + usuarios[i].carrera);
                                Console.WriteLine("Correo:   " + usuarios[i].correoElectronico);
                                Console.WriteLine("Teléfono: " + usuarios[i].telefono);
                                Console.WriteLine("Estado:   " + (usuarios[i].estado ? "Activo" : "Inactivo"));
                                Console.WriteLine("----------------------------------------");
                                encontradoU = true;
                                break;
                            }
                        }
                        if (!encontradoU)
                            Console.WriteLine("Usuario no encontrado.");
                        Console.ReadKey();
                        break;

                    case 3:
                        Console.Clear();
                        Console.WriteLine("Listado Completo de Usuarios");
                        if (cantUsuarios == 0)
                        {
                            Console.WriteLine("No hay usuarios registrados.");
                        }
                        else
                        {
                            for (int i = 0; i < cantUsuarios; i++)
                            {
                                Console.WriteLine("----------------------------------------");
                                Console.WriteLine("Carnet:   " + usuarios[i].carnet);
                                Console.WriteLine("Nombre:   " + usuarios[i].nombreCompleto);
                                Console.WriteLine("Carrera:  " + usuarios[i].carrera);
                                Console.WriteLine("Correo:   " + usuarios[i].correoElectronico);
                                Console.WriteLine("Teléfono: " + usuarios[i].telefono);
                                Console.WriteLine("Estado:   " + (usuarios[i].estado ? "Activo" : "Inactivo"));
                            }
                            Console.WriteLine("----------------------------------------");
                            Console.WriteLine("Total de usuarios: " + cantUsuarios);
                        }
                        Console.ReadKey();
                        break;

                    case 4: break;
                    default:
                        Console.WriteLine("Opción no válida. Presione una tecla para continuar");
                        Console.ReadKey();
                        break;
                }
            } while (opcion != 4);
        }
        static void GestióndePréstamos()
        {
            int opcion = 0;
            do
            {
                Console.Clear();
                Console.WriteLine("Gestión de Préstamos");
                Console.WriteLine("1. Registrar nuevo préstamo");
                Console.WriteLine("2. Registrar devolución de libro");
                Console.WriteLine("3. Listado de préstamos activos");
                Console.WriteLine("4. Buscar préstamo por carnet de usuario");
                Console.WriteLine("5. Volver al menú principal");
                Console.Write("Seleccione una opción: ");
                opcion = int.Parse(Console.ReadLine());
                switch (opcion)
                {
                    case 1:
                        Console.Clear();
                        Console.WriteLine("Registrar Nuevo Préstamo");
                        if (cantPrestamos >= prestamos.Length)
                        {
                            Console.WriteLine("No hay espacio para más préstamos.");
                            Console.ReadKey();
                            break;
                        }
                        Console.Write("Carnet del usuario: ");
                        string carnetP = Console.ReadLine();
                        bool usuarioValido = false;
                        bool usuarioActivo = false;
                        for (int i = 0; i < cantUsuarios; i++)
                        {
                            if (usuarios[i].carnet == carnetP)
                            {
                                usuarioValido = true;
                                if (usuarios[i].estado) usuarioActivo = true;
                                break;
                            }
                        }
                        if (!usuarioValido)
                        {
                            Console.WriteLine("Usuario no encontrado.");
                            Console.ReadKey();
                            break;
                        }
                        if (!usuarioActivo)
                        {
                            Console.WriteLine("El usuario está inactivo y no puede realizar préstamos.");
                            Console.ReadKey();
                            break;
                        }
                        Console.Write("Código del libro: ");
                        string codigoP = Console.ReadLine();
                        bool libroValido = false;
                        int posLibro = -1;
                        for (int i = 0; i < cantLibros; i++)
                        {
                            if (libros[i].codigo == codigoP)
                            {
                                libroValido = true;
                                posLibro = i;
                                break;
                            }
                        }
                        if (!libroValido)
                        {
                            Console.WriteLine("Libro no encontrado.");
                            Console.ReadKey();
                            break;
                        }
                        if (libros[posLibro].cantidadDeEjemplaresDisponibles <= 0)
                        {
                            Console.WriteLine("No hay ejemplares disponibles de este libro.");
                            Console.ReadKey();
                            break;
                        }
                        prestamos[cantPrestamos].carneUsuario = carnetP;
                        prestamos[cantPrestamos].codigoLibro = codigoP;
                        prestamos[cantPrestamos].fechaPrestamo = LeerFecha("Fecha de préstamo (dd/mm/yyyy): ");
                        prestamos[cantPrestamos].fechaDevolucionEstimada = LeerFecha("Fecha de devolución estimada (dd/mm/yyyy): ");
                        prestamos[cantPrestamos].estado = "Activo";
                        libros[posLibro].cantidadDeEjemplaresDisponibles--;
                        cantPrestamos++;
                        GuardarDatos();
                        Console.WriteLine("Préstamo registrado exitosamente.");
                        AbrirArchivo(dataArchivo + "prestamos.txt");
                        Console.ReadKey();
                        break;

                    case 2:
                        Console.Clear();
                        Console.WriteLine("Registrar Devolución");
                        Console.Write("Carnet del usuario: ");
                        string carnetDev = Console.ReadLine();
                        Console.Write("Código del libro: ");
                        string codigoDev = Console.ReadLine();
                        bool devEncontrado = false;
                        for (int i = 0; i < cantPrestamos; i++)
                        {
                            if (prestamos[i].carneUsuario == carnetDev &&
                                prestamos[i].codigoLibro == codigoDev &&
                                prestamos[i].estado == "Activo")
                            {
                                prestamos[i].estado = "Devuelto";
                                for (int j = 0; j < cantLibros; j++)
                                {
                                    if (libros[j].codigo == codigoDev)
                                    {
                                        libros[j].cantidadDeEjemplaresDisponibles++;
                                        break;
                                    }
                                }
                                GuardarDatos();
                                Console.WriteLine("Devolución registrada exitosamente.");
                                AbrirArchivo(dataArchivo + "prestamos.txt");
                                devEncontrado = true;
                                break;
                            }
                        }
                        if (!devEncontrado)
                            Console.WriteLine("No se encontró un préstamo activo con esos datos.");
                        Console.ReadKey();
                        break;

                    case 3:
                        Console.Clear();
                        Console.WriteLine("Préstamos Activos");
                        bool hayActivos = false;
                        for (int i = 0; i < cantPrestamos; i++)
                        {
                            if (prestamos[i].estado == "Activo")
                            {
                                Console.WriteLine("Carnet usuario:      " + prestamos[i].carneUsuario);
                                Console.WriteLine("Código libro:        " + prestamos[i].codigoLibro);
                                Console.WriteLine("Fecha préstamo:      " + prestamos[i].fechaPrestamo);
                                Console.WriteLine("Fecha devolución:    " + prestamos[i].fechaDevolucionEstimada);
                                Console.WriteLine("Estado:              " + prestamos[i].estado);
                                hayActivos = true;
                            }
                        }
                        if (!hayActivos) Console.WriteLine("No hay préstamos activos.");
                        Console.ReadKey();
                        break;

                    case 4:
                        Console.Clear();
                        Console.WriteLine("Búsqueda de Préstamos por Carnet");
                        Console.Write("Ingrese el carnet del usuario: ");
                        string carnetBuscarP = Console.ReadLine();
                        bool hayPrestamos = false;
                        for (int i = 0; i < cantPrestamos; i++)
                        {
                            if (prestamos[i].carneUsuario == carnetBuscarP)
                            {
                                Console.WriteLine("Código libro:        " + prestamos[i].codigoLibro);
                                Console.WriteLine("Fecha préstamo:      " + prestamos[i].fechaPrestamo);
                                Console.WriteLine("Fecha devolución:    " + prestamos[i].fechaDevolucionEstimada);
                                Console.WriteLine("Estado:              " + prestamos[i].estado);
                                hayPrestamos = true;
                            }
                        }
                        if (!hayPrestamos) Console.WriteLine("No se encontraron préstamos para ese carnet.");
                        Console.ReadKey();
                        break;

                    case 5: break;
                    default:
                        Console.WriteLine("Opción no válida. Presione una tecla para continuar...");
                        Console.ReadKey();
                        break;
                }
            } while (opcion != 5);
        }
    }

}
