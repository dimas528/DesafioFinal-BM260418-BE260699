// ============================================================
// SISTEMA INTEGRAL DE GESTIÓN DE BIBLIOTECA UNIVERSITARIA
// Universidad Don Bosco
// Programación de Algoritmos - Desafío Final Ciclo I 2026
// ============================================================
// Estudiante: [Tu Nombre]
// Carné: [Tu Carné]
// ============================================================

using System;
using System.IO;

namespace SistemaBibliotecaUDB
{
    class Program
    {
        // =====================================================
        // DEFINICIÓN DE STRUCTS
        // =====================================================

        struct Libro
        {
            public string Codigo;           // 8 chars alfanuméricos (ej. LIB00001)
            public string Titulo;
            public string Autor;
            public string Editorial;
            public int    AnoPublicacion;   // entre 1900 y año actual
            public string Categoria;
            public int    CantidadEjemplares; // no negativo
        }

        struct Usuario
        {
            public string Carne;            // 8 dígitos numéricos
            public string NombreCompleto;
            public string Carrera;
            public string Correo;           // debe contener '@' y punto posterior
            public string Telefono;
            public string Estado;           // "activo" / "inactivo"
        }

        struct Prestamo
        {
            public string Id;               // P001, P002, ...
            public string CarneUsuario;
            public string CodigoLibro;
            public string FechaPrestamo;            // dd/mm/yyyy
            public string FechaDevolucionEstimada;  // dd/mm/yyyy
            public string Estado;           // "activo" / "devuelto"
        }

        // =====================================================
        // ARREGLOS GLOBALES (máximos según requerimientos)
        // =====================================================
        static Libro[]   libros   = new Libro[10];
        static Usuario[] usuarios = new Usuario[5];
        static Prestamo[] prestamos = new Prestamo[10];

        static int cantLibros   = 0;
        static int cantUsuarios = 0;
        static int cantPrestamos = 0;

        // =====================================================
        // RUTAS DE ARCHIVOS
        // =====================================================
        static readonly string CarpetaData     = "Data";
        static readonly string ArchivoLibros   = Path.Combine("Data", "libros.csv");
        static readonly string ArchivoUsuarios = Path.Combine("Data", "usuarios.txt");
        static readonly string ArchivoPrestamos= Path.Combine("Data", "prestamos.txt");

        // =====================================================
        // PUNTO DE ENTRADA
        // =====================================================
        static void Main(string[] args)
        {
            ConfigurarConsola();
            CrearCarpetaData();
            CargarDatos();          // carga automática al iniciar

            string opcion;
            do
            {
                MostrarMenuPrincipal();
                opcion = Console.ReadLine();

                switch (opcion)
                {
                    case "1": MenuGestionLibros();    break;
                    case "2": MenuGestionUsuarios();  break;
                    case "3": MenuGestionPrestamos(); break;
                    case "4":
                        GuardarDatos();             // guardado automático al salir
                        MostrarDespedida();
                        break;
                    default:
                        MostrarError("Opción no válida. Intente de nuevo.");
                        Pausar();
                        break;
                }
            } while (opcion != "4");
        }

        // =====================================================
        // CONFIGURACIÓN INICIAL
        // =====================================================

        /// <summary>Configura la consola: título, colores y tamaño.</summary>
        static void ConfigurarConsola()
        {
            Console.Title = "Biblioteca UDB - Sistema de Gestión";
            try
            {
                Console.WindowHeight = 42;
                Console.WindowWidth  = 80;
            }
            catch { /* en Linux/Mac el tamaño de ventana puede no soportarse */ }
        }

        /// <summary>Crea la carpeta Data/ si no existe.</summary>
        static void CrearCarpetaData()
        {
            if (!Directory.Exists(CarpetaData))
                Directory.CreateDirectory(CarpetaData);
        }

        // =====================================================
        // MENÚ PRINCIPAL
        // =====================================================

        static void MostrarMenuPrincipal()
        {
            Console.Clear();
            Encabezado("SISTEMA DE GESTIÓN DE BIBLIOTECA UNIVERSITARIA");
            Console.WriteLine("  [1] Gestión de Libros");
            Console.WriteLine("  [2] Gestión de Usuarios");
            Console.WriteLine("  [3] Gestión de Préstamos");
            Console.WriteLine("  [4] Salir del Sistema");
            Linea();
            Console.Write("  Seleccione una opción: ");
        }

        // =====================================================
        // MÓDULO A – GESTIÓN DE LIBROS
        // =====================================================

        static void MenuGestionLibros()
        {
            string opcion;
            do
            {
                Console.Clear();
                Encabezado("MÓDULO A – GESTIÓN DE LIBROS");
                Console.WriteLine("  [1] Registrar nuevo libro");
                Console.WriteLine("  [2] Buscar libro por código");
                Console.WriteLine("  [3] Listar todos los libros");
                Console.WriteLine("  [4] Eliminar libro");
                Console.WriteLine("  [5] Estadísticas de inventario (matriz)");
                Console.WriteLine("  [6] Guardar libros manualmente");
                Console.WriteLine("  [7] Volver al menú principal");
                Linea();
                Console.Write("  Seleccione una opción: ");
                opcion = Console.ReadLine();

                switch (opcion)
                {
                    case "1": RegistrarLibro();          break;
                    case "2": BuscarLibroCodigo();       break;
                    case "3": ListarLibros();             break;
                    case "4": EliminarLibro();            break;
                    case "5": EstadisticasMatrizLibros(); break;
                    case "6": GuardarLibros(); MostrarExito("Libros guardados."); Pausar(); break;
                    case "7": break;
                    default:
                        MostrarError("Opción no válida.");
                        Pausar();
                        break;
                }
            } while (opcion != "7");
        }

        /// <summary>Registra un nuevo libro solicitando todos sus campos con validación.</summary>
        static void RegistrarLibro()
        {
            Console.Clear();
            Encabezado("REGISTRAR NUEVO LIBRO");

            if (cantLibros >= 10)
            {
                MostrarError("Capacidad máxima de libros alcanzada (10).");
                Pausar();
                return;
            }

            Libro l = new Libro();

            // --- Código (8 chars alfanuméricos) ---
            do
            {
                Console.Write("  Código (8 chars alfanuméricos, ej. LIB00001): ");
                l.Codigo = Console.ReadLine().Trim().ToUpper();
                if (!ValidarCodigoLibro(l.Codigo))
                    MostrarError("Código inválido. Exactamente 8 caracteres alfanuméricos.");
            } while (!ValidarCodigoLibro(l.Codigo));

            if (BuscarIndiceLibro(l.Codigo) >= 0)
            {
                MostrarError("Ya existe un libro con ese código.");
                Pausar();
                return;
            }

            // --- Título ---
            l.Titulo = LeerCadenaObligatoria("  Título: ");

            // --- Autor ---
            l.Autor = LeerCadenaObligatoria("  Autor: ");

            // --- Editorial ---
            l.Editorial = LeerCadenaObligatoria("  Editorial: ");

            // --- Año de publicación ---
            l.AnoPublicacion = LeerEnteroEnRango(
                $"  Año de publicación (1900-{DateTime.Now.Year}): ",
                1900, DateTime.Now.Year);

            // --- Categoría ---
            l.Categoria = LeerCadenaObligatoria("  Categoría: ");

            // --- Ejemplares ---
            l.CantidadEjemplares = LeerEnteroNoNegativo("  Cantidad de ejemplares: ");

            libros[cantLibros] = l;
            cantLibros++;

            MostrarExito($"Libro '{l.Titulo}' registrado exitosamente.");
            Pausar();
        }

        /// <summary>Busca un libro por su código y muestra sus datos.</summary>
        static void BuscarLibroCodigo()
        {
            Console.Clear();
            Encabezado("BUSCAR LIBRO POR CÓDIGO");
            Console.Write("  Ingrese el código: ");
            string cod = Console.ReadLine().Trim().ToUpper();

            int idx = BuscarIndiceLibro(cod);
            if (idx >= 0)
                DetalleLibro(libros[idx]);
            else
                MostrarError("No se encontró ningún libro con ese código.");

            Pausar();
        }

        /// <summary>Muestra todos los libros registrados en memoria.</summary>
        static void ListarLibros()
        {
            Console.Clear();
            Encabezado("LISTADO DE LIBROS");

            if (cantLibros == 0)
            {
                MostrarError("No hay libros registrados.");
            }
            else
            {
                for (int i = 0; i < cantLibros; i++)
                    DetalleLibro(libros[i]);
                Console.WriteLine($"  Total de libros: {cantLibros}");
            }
            Pausar();
        }

        /// <summary>Elimina un libro del arreglo mediante desplazamiento.</summary>
        static void EliminarLibro()
        {
            Console.Clear();
            Encabezado("ELIMINAR LIBRO");
            Console.Write("  Ingrese el código del libro a eliminar: ");
            string cod = Console.ReadLine().Trim().ToUpper();

            int idx = BuscarIndiceLibro(cod);
            if (idx < 0)
            {
                MostrarError("No se encontró ningún libro con ese código.");
                Pausar();
                return;
            }

            DetalleLibro(libros[idx]);
            Console.Write("  ¿Confirma la eliminación? (S/N): ");
            string conf = Console.ReadLine().Trim().ToUpper();

            if (conf == "S")
            {
                // Desplazar elementos (estructura de repetición for)
                for (int i = idx; i < cantLibros - 1; i++)
                    libros[i] = libros[i + 1];
                cantLibros--;
                MostrarExito("Libro eliminado exitosamente.");
            }
            else
            {
                Console.WriteLine("  Operación cancelada.");
            }
            Pausar();
        }

        /// <summary>
        /// Genera una MATRIZ de estadísticas: filas = categorías únicas,
        /// columnas = [con ejemplares, sin ejemplares, total].
        /// Demuestra el uso obligatorio de matrices (arreglos 2D).
        /// </summary>
        static void EstadisticasMatrizLibros()
        {
            Console.Clear();
            Encabezado("ESTADÍSTICAS DE INVENTARIO (MATRIZ)");

            if (cantLibros == 0)
            {
                MostrarError("No hay libros para generar estadísticas.");
                Pausar();
                return;
            }

            // Recolectar categorías únicas con while
            string[] categorias = new string[10];
            int numCategorias = 0;
            int i = 0;
            while (i < cantLibros)
            {
                bool existe = false;
                for (int j = 0; j < numCategorias; j++)
                {
                    if (categorias[j].Equals(libros[i].Categoria,
                        StringComparison.OrdinalIgnoreCase))
                    {
                        existe = true;
                        break;
                    }
                }
                if (!existe && numCategorias < 10)
                {
                    categorias[numCategorias] = libros[i].Categoria;
                    numCategorias++;
                }
                i++;
            }

            // Matriz [categoria][0=conEjemplares, 1=sinEjemplares, 2=total]
            int[,] matriz = new int[numCategorias, 3];

            // Llenar la matriz
            for (int f = 0; f < cantLibros; f++)
            {
                for (int c = 0; c < numCategorias; c++)
                {
                    if (libros[f].Categoria.Equals(categorias[c],
                        StringComparison.OrdinalIgnoreCase))
                    {
                        if (libros[f].CantidadEjemplares > 0)
                            matriz[c, 0]++;   // con ejemplares
                        else
                            matriz[c, 1]++;   // sin ejemplares
                        matriz[c, 2]++;       // total
                        break;
                    }
                }
            }

            // Mostrar la matriz
            Console.WriteLine("  {0,-20} {1,15} {2,15} {3,10}",
                "Categoría", "Disponibles", "Sin Stock", "Total");
            Console.WriteLine("  " + new string('-', 62));

            for (int r = 0; r < numCategorias; r++)
            {
                Console.WriteLine("  {0,-20} {1,15} {2,15} {3,10}",
                    categorias[r], matriz[r, 0], matriz[r, 1], matriz[r, 2]);
            }

            Console.WriteLine("  " + new string('-', 62));
            Console.WriteLine($"  Total de libros registrados: {cantLibros}");
            Pausar();
        }

        // =====================================================
        // MÓDULO B – GESTIÓN DE USUARIOS
        // =====================================================

        static void MenuGestionUsuarios()
        {
            string opcion;
            do
            {
                Console.Clear();
                Encabezado("MÓDULO B – GESTIÓN DE USUARIOS");
                Console.WriteLine("  [1] Registrar nuevo usuario");
                Console.WriteLine("  [2] Buscar usuario por carné");
                Console.WriteLine("  [3] Buscar usuario por nombre");
                Console.WriteLine("  [4] Listar todos los usuarios");
                Console.WriteLine("  [5] Cambiar estado de usuario");
                Console.WriteLine("  [6] Volver al menú principal");
                Linea();
                Console.Write("  Seleccione una opción: ");
                opcion = Console.ReadLine();

                switch (opcion)
                {
                    case "1": RegistrarUsuario();      break;
                    case "2": BuscarUsuarioCarne();    break;
                    case "3": BuscarUsuarioNombre();   break;
                    case "4": ListarUsuarios();        break;
                    case "5": CambiarEstadoUsuario();  break;
                    case "6": break;
                    default:
                        MostrarError("Opción no válida.");
                        Pausar();
                        break;
                }
            } while (opcion != "6");
        }

        /// <summary>Registra un nuevo usuario con todos sus campos validados.</summary>
        static void RegistrarUsuario()
        {
            Console.Clear();
            Encabezado("REGISTRAR NUEVO USUARIO");

            if (cantUsuarios >= 5)
            {
                MostrarError("Capacidad máxima de usuarios alcanzada (5).");
                Pausar();
                return;
            }

            Usuario u = new Usuario();

            // --- Carné (8 dígitos) ---
            do
            {
                Console.Write("  Carné (8 dígitos numéricos): ");
                u.Carne = Console.ReadLine().Trim();
                if (!ValidarCarne(u.Carne))
                    MostrarError("Carné inválido. Debe tener exactamente 8 dígitos.");
            } while (!ValidarCarne(u.Carne));

            if (BuscarIndiceUsuario(u.Carne) >= 0)
            {
                MostrarError("Ya existe un usuario con ese carné.");
                Pausar();
                return;
            }

            // --- Nombre ---
            u.NombreCompleto = LeerCadenaObligatoria("  Nombre completo: ");

            // --- Carrera ---
            u.Carrera = LeerCadenaObligatoria("  Carrera: ");

            // --- Correo ---
            do
            {
                Console.Write("  Correo electrónico: ");
                u.Correo = Console.ReadLine().Trim();
                if (!ValidarCorreo(u.Correo))
                    MostrarError("Correo inválido. Debe contener '@' y un punto posterior.");
            } while (!ValidarCorreo(u.Correo));

            // --- Teléfono ---
            u.Telefono = LeerCadenaObligatoria("  Teléfono: ");

            u.Estado = "activo";

            usuarios[cantUsuarios] = u;
            cantUsuarios++;

            MostrarExito($"Usuario '{u.NombreCompleto}' registrado exitosamente.");
            Pausar();
        }

        /// <summary>Busca un usuario por su carné y muestra sus datos.</summary>
        static void BuscarUsuarioCarne()
        {
            Console.Clear();
            Encabezado("BUSCAR USUARIO POR CARNÉ");
            Console.Write("  Ingrese el carné: ");
            string carne = Console.ReadLine().Trim();

            int idx = BuscarIndiceUsuario(carne);
            if (idx >= 0)
                DetalleUsuario(usuarios[idx]);
            else
                MostrarError("No se encontró ningún usuario con ese carné.");

            Pausar();
        }

        /// <summary>Busca usuarios cuyo nombre contenga la cadena ingresada (búsqueda parcial).</summary>
        static void BuscarUsuarioNombre()
        {
            Console.Clear();
            Encabezado("BUSCAR USUARIO POR NOMBRE");
            Console.Write("  Ingrese el nombre (o parte): ");
            string nombre = Console.ReadLine().Trim().ToLower();

            bool encontrado = false;
            for (int i = 0; i < cantUsuarios; i++)
            {
                if (usuarios[i].NombreCompleto.ToLower().Contains(nombre))
                {
                    DetalleUsuario(usuarios[i]);
                    encontrado = true;
                }
            }

            if (!encontrado)
                MostrarError("No se encontró ningún usuario con ese nombre.");

            Pausar();
        }

        /// <summary>Lista todos los usuarios registrados en memoria.</summary>
        static void ListarUsuarios()
        {
            Console.Clear();
            Encabezado("LISTADO DE USUARIOS");

            if (cantUsuarios == 0)
            {
                MostrarError("No hay usuarios registrados.");
            }
            else
            {
                for (int i = 0; i < cantUsuarios; i++)
                    DetalleUsuario(usuarios[i]);
                Console.WriteLine($"  Total de usuarios: {cantUsuarios}");
            }
            Pausar();
        }

        /// <summary>Permite cambiar el estado de un usuario entre activo e inactivo.</summary>
        static void CambiarEstadoUsuario()
        {
            Console.Clear();
            Encabezado("CAMBIAR ESTADO DE USUARIO");
            Console.Write("  Ingrese el carné del usuario: ");
            string carne = Console.ReadLine().Trim();

            int idx = BuscarIndiceUsuario(carne);
            if (idx < 0)
            {
                MostrarError("Usuario no encontrado.");
                Pausar();
                return;
            }

            DetalleUsuario(usuarios[idx]);
            Console.WriteLine("  Estado actual: " + usuarios[idx].Estado);
            Console.WriteLine("  [1] Activo    [2] Inactivo");
            Console.Write("  Seleccione: ");
            string op = Console.ReadLine().Trim();

            if (op == "1")
            {
                usuarios[idx].Estado = "activo";
                MostrarExito("Estado cambiado a 'activo'.");
            }
            else if (op == "2")
            {
                usuarios[idx].Estado = "inactivo";
                MostrarExito("Estado cambiado a 'inactivo'.");
            }
            else
            {
                MostrarError("Opción no válida.");
            }
            Pausar();
        }

        // =====================================================
        // MÓDULO C – GESTIÓN DE PRÉSTAMOS
        // =====================================================

        static void MenuGestionPrestamos()
        {
            string opcion;
            do
            {
                Console.Clear();
                Encabezado("MÓDULO C – GESTIÓN DE PRÉSTAMOS");
                Console.WriteLine("  [1] Registrar préstamo");
                Console.WriteLine("  [2] Registrar devolución");
                Console.WriteLine("  [3] Consultar préstamos activos de un usuario");
                Console.WriteLine("  [4] Actualizar estado de préstamo");
                Console.WriteLine("  [5] Listar todos los préstamos");
                Console.WriteLine("  [6] Exportar reporte a archivo .txt");
                Console.WriteLine("  [7] Volver al menú principal");
                Linea();
                Console.Write("  Seleccione una opción: ");
                opcion = Console.ReadLine();

                switch (opcion)
                {
                    case "1": RegistrarPrestamo();               break;
                    case "2": RegistrarDevolucion();             break;
                    case "3": ConsultarPrestamosActivos();       break;
                    case "4": ActualizarEstadoPrestamo();        break;
                    case "5": ListarPrestamos();                 break;
                    case "6": ExportarReportePrestamos();        break;
                    case "7": break;
                    default:
                        MostrarError("Opción no válida.");
                        Pausar();
                        break;
                }
            } while (opcion != "7");
        }

        /// <summary>
        /// Registra un nuevo préstamo vinculando usuario con libro,
        /// valida disponibilidad y descuenta del inventario.
        /// </summary>
        static void RegistrarPrestamo()
        {
            Console.Clear();
            Encabezado("REGISTRAR PRÉSTAMO");

            if (cantPrestamos >= 10)
            {
                MostrarError("Capacidad máxima de préstamos alcanzada (10).");
                Pausar();
                return;
            }

            // Validar usuario
            Console.Write("  Carné del usuario: ");
            string carne = Console.ReadLine().Trim();
            int idxU = BuscarIndiceUsuario(carne);
            if (idxU < 0)
            {
                MostrarError("Usuario no encontrado.");
                Pausar();
                return;
            }
            if (usuarios[idxU].Estado != "activo")
            {
                MostrarError("El usuario está inactivo y no puede realizar préstamos.");
                Pausar();
                return;
            }

            // Validar libro
            Console.Write("  Código del libro: ");
            string cod = Console.ReadLine().Trim().ToUpper();
            int idxL = BuscarIndiceLibro(cod);
            if (idxL < 0)
            {
                MostrarError("Libro no encontrado.");
                Pausar();
                return;
            }
            if (libros[idxL].CantidadEjemplares <= 0)
            {
                MostrarError("No hay ejemplares disponibles de ese libro.");
                Pausar();
                return;
            }

            Prestamo p = new Prestamo();
            p.CarneUsuario = carne;
            p.CodigoLibro  = cod;

            // Fechas
            do
            {
                Console.Write("  Fecha de préstamo (dd/mm/yyyy): ");
                p.FechaPrestamo = Console.ReadLine().Trim();
                if (!ValidarFecha(p.FechaPrestamo))
                    MostrarError("Formato inválido. Use dd/mm/yyyy.");
            } while (!ValidarFecha(p.FechaPrestamo));

            do
            {
                Console.Write("  Fecha estimada de devolución (dd/mm/yyyy): ");
                p.FechaDevolucionEstimada = Console.ReadLine().Trim();
                if (!ValidarFecha(p.FechaDevolucionEstimada))
                    MostrarError("Formato inválido. Use dd/mm/yyyy.");
            } while (!ValidarFecha(p.FechaDevolucionEstimada));

            p.Estado = "activo";
            p.Id     = "P" + (cantPrestamos + 1).ToString("D3"); // P001, P002...

            // Descontar ejemplar del inventario
            libros[idxL].CantidadEjemplares--;

            prestamos[cantPrestamos] = p;
            cantPrestamos++;

            MostrarExito($"Préstamo registrado. ID: {p.Id} | Ejemplares restantes: {libros[idxL].CantidadEjemplares}");
            Pausar();
        }

        /// <summary>Registra la devolución de un libro y actualiza el inventario.</summary>
        static void RegistrarDevolucion()
        {
            Console.Clear();
            Encabezado("REGISTRAR DEVOLUCIÓN");
            Console.Write("  ID del préstamo (ej. P001): ");
            string id = Console.ReadLine().Trim().ToUpper();

            int idx = BuscarIndicePrestamo(id);
            if (idx < 0)
            {
                MostrarError("Préstamo no encontrado.");
                Pausar();
                return;
            }
            if (prestamos[idx].Estado == "devuelto")
            {
                MostrarError("Este préstamo ya fue marcado como devuelto.");
                Pausar();
                return;
            }

            DetallePrestamo(prestamos[idx]);

            // Actualizar inventario del libro
            int idxL = BuscarIndiceLibro(prestamos[idx].CodigoLibro);
            if (idxL >= 0)
                libros[idxL].CantidadEjemplares++;

            prestamos[idx].Estado = "devuelto";
            MostrarExito("Devolución registrada. Inventario actualizado.");
            Pausar();
        }

        /// <summary>Muestra todos los préstamos activos de un usuario específico.</summary>
        static void ConsultarPrestamosActivos()
        {
            Console.Clear();
            Encabezado("PRÉSTAMOS ACTIVOS DE UN USUARIO");
            Console.Write("  Ingrese el carné del usuario: ");
            string carne = Console.ReadLine().Trim();

            bool encontrado = false;
            // Uso de for para recorrer préstamos
            for (int i = 0; i < cantPrestamos; i++)
            {
                if (prestamos[i].CarneUsuario == carne && prestamos[i].Estado == "activo")
                {
                    DetallePrestamo(prestamos[i]);
                    encontrado = true;
                }
            }

            if (!encontrado)
                MostrarError("No se encontraron préstamos activos para ese usuario.");

            Pausar();
        }

        /// <summary>Permite actualizar el estado de un préstamo existente.</summary>
        static void ActualizarEstadoPrestamo()
        {
            Console.Clear();
            Encabezado("ACTUALIZAR ESTADO DE PRÉSTAMO");
            Console.Write("  ID del préstamo: ");
            string id = Console.ReadLine().Trim().ToUpper();

            int idx = BuscarIndicePrestamo(id);
            if (idx < 0)
            {
                MostrarError("Préstamo no encontrado.");
                Pausar();
                return;
            }

            DetallePrestamo(prestamos[idx]);
            Console.WriteLine("  [1] activo    [2] devuelto");
            Console.Write("  Seleccione nuevo estado: ");
            string op = Console.ReadLine().Trim();

            switch (op)
            {
                case "1":
                    prestamos[idx].Estado = "activo";
                    MostrarExito("Estado actualizado a 'activo'.");
                    break;
                case "2":
                    // Solo devuelve al inventario si antes estaba activo
                    if (prestamos[idx].Estado == "activo")
                    {
                        int idxL = BuscarIndiceLibro(prestamos[idx].CodigoLibro);
                        if (idxL >= 0) libros[idxL].CantidadEjemplares++;
                    }
                    prestamos[idx].Estado = "devuelto";
                    MostrarExito("Estado actualizado a 'devuelto'. Inventario actualizado.");
                    break;
                default:
                    MostrarError("Opción no válida.");
                    break;
            }
            Pausar();
        }

        /// <summary>Lista todos los préstamos registrados.</summary>
        static void ListarPrestamos()
        {
            Console.Clear();
            Encabezado("LISTADO DE PRÉSTAMOS");

            if (cantPrestamos == 0)
            {
                MostrarError("No hay préstamos registrados.");
            }
            else
            {
                for (int i = 0; i < cantPrestamos; i++)
                    DetallePrestamo(prestamos[i]);
                Console.WriteLine($"  Total de préstamos: {cantPrestamos}");
            }
            Pausar();
        }

        /// <summary>Exporta un reporte completo de préstamos a un archivo .txt.</summary>
        static void ExportarReportePrestamos()
        {
            Console.Clear();
            Encabezado("EXPORTAR REPORTE DE PRÉSTAMOS");
            string rutaReporte = Path.Combine(CarpetaData, "reporte_prestamos.txt");

            try
            {
                StreamWriter sw = new StreamWriter(rutaReporte, false);

                sw.WriteLine("================================================");
                sw.WriteLine("   REPORTE DE PRÉSTAMOS - BIBLIOTECA UDB");
                sw.WriteLine($"   Generado: {DateTime.Now:dd/MM/yyyy HH:mm:ss}");
                sw.WriteLine("================================================");
                sw.WriteLine();

                int activos = 0, devueltos = 0;

                // Ciclo for para recorrer y escribir cada préstamo
                for (int i = 0; i < cantPrestamos; i++)
                {
                    sw.WriteLine($"  ID Préstamo    : {prestamos[i].Id}");
                    sw.WriteLine($"  Carné Usuario  : {prestamos[i].CarneUsuario}");
                    sw.WriteLine($"  Código Libro   : {prestamos[i].CodigoLibro}");
                    sw.WriteLine($"  Fecha Préstamo : {prestamos[i].FechaPrestamo}");
                    sw.WriteLine($"  Fecha Devol.   : {prestamos[i].FechaDevolucionEstimada}");
                    sw.WriteLine($"  Estado         : {prestamos[i].Estado}");
                    sw.WriteLine("  " + new string('-', 44));

                    if (prestamos[i].Estado == "activo") activos++;
                    else devueltos++;
                }

                sw.WriteLine();
                sw.WriteLine("  ── RESUMEN ──");
                sw.WriteLine($"  Total préstamos  : {cantPrestamos}");
                sw.WriteLine($"  Activos          : {activos}");
                sw.WriteLine($"  Devueltos        : {devueltos}");
                sw.Close();

                MostrarExito($"Reporte exportado exitosamente a: {rutaReporte}");
            }
            catch (Exception ex)
            {
                MostrarError("Error al exportar reporte: " + ex.Message);
            }
            Pausar();
        }

        // =====================================================
        // MANEJO DE ARCHIVOS – GUARDAR
        // =====================================================

        /// <summary>Guarda todos los módulos de datos en sus respectivos archivos.</summary>
        static void GuardarDatos()
        {
            GuardarLibros();
            GuardarUsuarios();
            GuardarPrestamos();
        }

        /// <summary>Escribe los libros en libros.csv con separador ';'.</summary>
        static void GuardarLibros()
        {
            try
            {
                StreamWriter sw = new StreamWriter(ArchivoLibros, false);
                for (int i = 0; i < cantLibros; i++)
                {
                    sw.WriteLine(
                        libros[i].Codigo        + ";" +
                        libros[i].Titulo        + ";" +
                        libros[i].Autor         + ";" +
                        libros[i].Editorial     + ";" +
                        libros[i].AnoPublicacion + ";" +
                        libros[i].Categoria     + ";" +
                        libros[i].CantidadEjemplares
                    );
                }
                sw.Close();
            }
            catch (Exception ex)
            {
                MostrarError("Error al guardar libros: " + ex.Message);
            }
        }

        /// <summary>Escribe los usuarios en usuarios.txt con separador '|'.</summary>
        static void GuardarUsuarios()
        {
            try
            {
                StreamWriter sw = new StreamWriter(ArchivoUsuarios, false);
                for (int i = 0; i < cantUsuarios; i++)
                {
                    sw.WriteLine(
                        usuarios[i].Carne           + "|" +
                        usuarios[i].NombreCompleto  + "|" +
                        usuarios[i].Carrera         + "|" +
                        usuarios[i].Correo          + "|" +
                        usuarios[i].Telefono        + "|" +
                        usuarios[i].Estado
                    );
                }
                sw.Close();
            }
            catch (Exception ex)
            {
                MostrarError("Error al guardar usuarios: " + ex.Message);
            }
        }

        /// <summary>Escribe los préstamos en prestamos.txt con separador '|'.</summary>
        static void GuardarPrestamos()
        {
            try
            {
                StreamWriter sw = new StreamWriter(ArchivoPrestamos, false);
                for (int i = 0; i < cantPrestamos; i++)
                {
                    sw.WriteLine(
                        prestamos[i].Id                     + "|" +
                        prestamos[i].CarneUsuario           + "|" +
                        prestamos[i].CodigoLibro            + "|" +
                        prestamos[i].FechaPrestamo          + "|" +
                        prestamos[i].FechaDevolucionEstimada + "|" +
                        prestamos[i].Estado
                    );
                }
                sw.Close();
            }
            catch (Exception ex)
            {
                MostrarError("Error al guardar préstamos: " + ex.Message);
            }
        }

        // =====================================================
        // MANEJO DE ARCHIVOS – CARGAR
        // =====================================================

        /// <summary>Carga todos los módulos de datos desde sus archivos.</summary>
        static void CargarDatos()
        {
            CargarLibros();
            CargarUsuarios();
            CargarPrestamos();
        }

        /// <summary>Lee libros.csv y llena el arreglo de libros.</summary>
        static void CargarLibros()
        {
            if (!File.Exists(ArchivoLibros)) return;
            try
            {
                StreamReader sr = new StreamReader(ArchivoLibros);
                cantLibros = 0;

                // Uso de while para leer línea a línea
                while (!sr.EndOfStream && cantLibros < 10)
                {
                    string linea = sr.ReadLine();
                    if (string.IsNullOrWhiteSpace(linea)) continue;

                    string[] p = linea.Split(';');
                    if (p.Length >= 7)
                    {
                        libros[cantLibros].Codigo             = p[0];
                        libros[cantLibros].Titulo             = p[1];
                        libros[cantLibros].Autor              = p[2];
                        libros[cantLibros].Editorial          = p[3];
                        libros[cantLibros].AnoPublicacion     = int.Parse(p[4]);
                        libros[cantLibros].Categoria          = p[5];
                        libros[cantLibros].CantidadEjemplares = int.Parse(p[6]);
                        cantLibros++;
                    }
                }
                sr.Close();
            }
            catch (Exception ex)
            {
                MostrarError("Error al cargar libros: " + ex.Message);
            }
        }

        /// <summary>Lee usuarios.txt y llena el arreglo de usuarios.</summary>
        static void CargarUsuarios()
        {
            if (!File.Exists(ArchivoUsuarios)) return;
            try
            {
                StreamReader sr = new StreamReader(ArchivoUsuarios);
                cantUsuarios = 0;

                while (!sr.EndOfStream && cantUsuarios < 5)
                {
                    string linea = sr.ReadLine();
                    if (string.IsNullOrWhiteSpace(linea)) continue;

                    string[] p = linea.Split('|');
                    if (p.Length >= 6)
                    {
                        usuarios[cantUsuarios].Carne          = p[0];
                        usuarios[cantUsuarios].NombreCompleto = p[1];
                        usuarios[cantUsuarios].Carrera        = p[2];
                        usuarios[cantUsuarios].Correo         = p[3];
                        usuarios[cantUsuarios].Telefono       = p[4];
                        usuarios[cantUsuarios].Estado         = p[5];
                        cantUsuarios++;
                    }
                }
                sr.Close();
            }
            catch (Exception ex)
            {
                MostrarError("Error al cargar usuarios: " + ex.Message);
            }
        }

        /// <summary>Lee prestamos.txt y llena el arreglo de préstamos.</summary>
        static void CargarPrestamos()
        {
            if (!File.Exists(ArchivoPrestamos)) return;
            try
            {
                StreamReader sr = new StreamReader(ArchivoPrestamos);
                cantPrestamos = 0;

                while (!sr.EndOfStream && cantPrestamos < 10)
                {
                    string linea = sr.ReadLine();
                    if (string.IsNullOrWhiteSpace(linea)) continue;

                    string[] p = linea.Split('|');
                    if (p.Length >= 6)
                    {
                        prestamos[cantPrestamos].Id                      = p[0];
                        prestamos[cantPrestamos].CarneUsuario            = p[1];
                        prestamos[cantPrestamos].CodigoLibro             = p[2];
                        prestamos[cantPrestamos].FechaPrestamo           = p[3];
                        prestamos[cantPrestamos].FechaDevolucionEstimada = p[4];
                        prestamos[cantPrestamos].Estado                  = p[5];
                        cantPrestamos++;
                    }
                }
                sr.Close();
            }
            catch (Exception ex)
            {
                MostrarError("Error al cargar préstamos: " + ex.Message);
            }
        }

        // =====================================================
        // MÉTODOS DE VALIDACIÓN
        // =====================================================

        /// <summary>Valida que el código de libro tenga exactamente 8 caracteres alfanuméricos.</summary>
        static bool ValidarCodigoLibro(string codigo)
        {
            if (codigo.Length != 8) return false;
            for (int i = 0; i < codigo.Length; i++)
                if (!char.IsLetterOrDigit(codigo[i])) return false;
            return true;
        }

        /// <summary>Valida que el carné tenga exactamente 8 dígitos numéricos.</summary>
        static bool ValidarCarne(string carne)
        {
            if (carne.Length != 8) return false;
            for (int i = 0; i < carne.Length; i++)
                if (!char.IsDigit(carne[i])) return false;
            return true;
        }

        /// <summary>Valida que el correo contenga '@' y un punto posterior al mismo.</summary>
        static bool ValidarCorreo(string correo)
        {
            if (string.IsNullOrEmpty(correo)) return false;
            int posArroba = correo.IndexOf('@');
            if (posArroba < 0) return false;
            int posPunto = correo.IndexOf('.', posArroba);
            return posPunto > posArroba;
        }

        /// <summary>Valida que la fecha tenga formato dd/mm/yyyy (longitud y separadores).</summary>
        static bool ValidarFecha(string fecha)
        {
            if (fecha.Length != 10) return false;
            if (fecha[2] != '/' || fecha[5] != '/') return false;
            // Validar que los demás caracteres sean dígitos
            for (int i = 0; i < fecha.Length; i++)
            {
                if (i == 2 || i == 5) continue;
                if (!char.IsDigit(fecha[i])) return false;
            }
            return true;
        }

        // =====================================================
        // MÉTODOS DE BÚSQUEDA (devuelven índice o -1)
        // =====================================================

        /// <summary>Retorna el índice del libro con el código dado, o -1 si no existe.</summary>
        static int BuscarIndiceLibro(string codigo)
        {
            for (int i = 0; i < cantLibros; i++)
                if (libros[i].Codigo == codigo) return i;
            return -1;
        }

        /// <summary>Retorna el índice del usuario con el carné dado, o -1 si no existe.</summary>
        static int BuscarIndiceUsuario(string carne)
        {
            for (int i = 0; i < cantUsuarios; i++)
                if (usuarios[i].Carne == carne) return i;
            return -1;
        }

        /// <summary>Retorna el índice del préstamo con el ID dado, o -1 si no existe.</summary>
        static int BuscarIndicePrestamo(string id)
        {
            for (int i = 0; i < cantPrestamos; i++)
                if (prestamos[i].Id == id) return i;
            return -1;
        }

        // =====================================================
        // MÉTODOS DE ENTRADA REUTILIZABLES
        // =====================================================

        /// <summary>Lee una cadena no vacía mostrando el prompt dado.</summary>
        static string LeerCadenaObligatoria(string prompt)
        {
            string valor;
            do
            {
                Console.Write(prompt);
                valor = Console.ReadLine().Trim();
                if (string.IsNullOrEmpty(valor))
                    MostrarError("Este campo no puede estar vacío.");
            } while (string.IsNullOrEmpty(valor));
            return valor;
        }

        /// <summary>Lee un entero dentro del rango [min, max] con manejo de excepciones.</summary>
        static int LeerEnteroEnRango(string prompt, int min, int max)
        {
            int valor = 0;
            bool valido = false;
            do
            {
                try
                {
                    Console.Write(prompt);
                    valor = int.Parse(Console.ReadLine());
                    if (valor >= min && valor <= max)
                        valido = true;
                    else
                        MostrarError($"El valor debe estar entre {min} y {max}.");
                }
                catch (FormatException)
                {
                    MostrarError("Ingrese un número entero válido.");
                }
            } while (!valido);
            return valor;
        }

        /// <summary>Lee un entero no negativo con manejo de excepciones.</summary>
        static int LeerEnteroNoNegativo(string prompt)
        {
            int valor = 0;
            bool valido = false;
            do
            {
                try
                {
                    Console.Write(prompt);
                    valor = int.Parse(Console.ReadLine());
                    if (valor >= 0)
                        valido = true;
                    else
                        MostrarError("El valor no puede ser negativo.");
                }
                catch (FormatException)
                {
                    MostrarError("Ingrese un número entero válido.");
                }
            } while (!valido);
            return valor;
        }

        // =====================================================
        // MÉTODOS DE VISUALIZACIÓN
        // =====================================================

        /// <summary>Muestra el encabezado estilizado de cada sección.</summary>
        static void Encabezado(string titulo)
        {
            Linea();
            Console.WriteLine($"  {titulo}");
            Linea();
        }

        /// <summary>Dibuja una línea separadora.</summary>
        static void Linea()
        {
            Console.WriteLine("  " + new string('═', 62));
        }

        /// <summary>Muestra los detalles completos de un libro.</summary>
        static void DetalleLibro(Libro l)
        {
            Console.WriteLine("  " + new string('─', 50));
            Console.WriteLine($"  Código        : {l.Codigo}");
            Console.WriteLine($"  Título        : {l.Titulo}");
            Console.WriteLine($"  Autor         : {l.Autor}");
            Console.WriteLine($"  Editorial     : {l.Editorial}");
            Console.WriteLine($"  Año           : {l.AnoPublicacion}");
            Console.WriteLine($"  Categoría     : {l.Categoria}");
            Console.WriteLine($"  Ejemplares    : {l.CantidadEjemplares}");
            Console.WriteLine("  " + new string('─', 50));
        }

        /// <summary>Muestra los detalles completos de un usuario.</summary>
        static void DetalleUsuario(Usuario u)
        {
            Console.WriteLine("  " + new string('─', 50));
            Console.WriteLine($"  Carné         : {u.Carne}");
            Console.WriteLine($"  Nombre        : {u.NombreCompleto}");
            Console.WriteLine($"  Carrera       : {u.Carrera}");
            Console.WriteLine($"  Correo        : {u.Correo}");
            Console.WriteLine($"  Teléfono      : {u.Telefono}");
            Console.WriteLine($"  Estado        : {u.Estado}");
            Console.WriteLine("  " + new string('─', 50));
        }

        /// <summary>Muestra los detalles completos de un préstamo.</summary>
        static void DetallePrestamo(Prestamo p)
        {
            Console.WriteLine("  " + new string('─', 50));
            Console.WriteLine($"  ID Préstamo   : {p.Id}");
            Console.WriteLine($"  Carné Usuario : {p.CarneUsuario}");
            Console.WriteLine($"  Código Libro  : {p.CodigoLibro}");
            Console.WriteLine($"  F. Préstamo   : {p.FechaPrestamo}");
            Console.WriteLine($"  F. Devolución : {p.FechaDevolucionEstimada}");
            Console.WriteLine($"  Estado        : {p.Estado}");
            Console.WriteLine("  " + new string('─', 50));
        }

        /// <summary>Muestra un mensaje de error en color rojo.</summary>
        static void MostrarError(string mensaje)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("  [!] " + mensaje);
            Console.ResetColor();
        }

        /// <summary>Muestra un mensaje de éxito en color verde.</summary>
        static void MostrarExito(string mensaje)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("  [✓] " + mensaje);
            Console.ResetColor();
        }

        /// <summary>Pausa la ejecución hasta que el usuario presione una tecla.</summary>
        static void Pausar()
        {
            Console.WriteLine();
            Console.Write("  Presione una tecla para continuar...");
            Console.ReadKey();
        }

        /// <summary>Muestra la pantalla de despedida al cerrar el sistema.</summary>
        static void MostrarDespedida()
        {
            Console.Clear();
            Console.WriteLine();
            Console.WriteLine("  ╔══════════════════════════════════════════════════╗");
            Console.WriteLine("  ║                                                  ║");
            Console.WriteLine("  ║   SISTEMA DE GESTIÓN DE BIBLIOTECA UDB           ║");
            Console.WriteLine("  ║   Datos guardados exitosamente.                  ║");
            Console.WriteLine("  ║                                                  ║");
            Console.WriteLine("  ║   Programación de Algoritmos – Ciclo I 2026      ║");
            Console.WriteLine("  ║   ¡Hasta pronto!                                ║");
            Console.WriteLine("  ║                                                  ║");
            Console.WriteLine("  ╚══════════════════════════════════════════════════╝");
            Console.WriteLine();
            Console.Write("  Presione una tecla para cerrar...");
            Console.ReadKey();
        }
    }
}
