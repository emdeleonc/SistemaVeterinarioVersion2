using System.Data;
using System.Globalization;
using SistemaVeterinario.Dominio;
using SistemaVeterinario.Persistencia;

namespace SistemaVeterinario.Aplicacion;

internal sealed class MenuVeterinario
{
    private readonly RepositorioVeterinario _repositorio;
    private bool _continuar = true;

    public MenuVeterinario(RepositorioVeterinario repositorio)
    {
        _repositorio = repositorio;
    }

    public void Ejecutar()
    {
        while (_continuar)
        {
            MostrarMenu();
            try
            {
                var entrada = Console.ReadLine();
                if (entrada is null)
                {
                    _continuar = false;
                    continue;
                }

                switch (entrada.Trim())
                {
                    case "1": CrearPropietario(); break;
                    case "2": ListarPropietarios(); break;
                    case "3": ActualizarPropietario(); break;
                    case "4": EliminarPropietario(); break;
                    case "5": CrearMascota(); break;
                    case "6": ListarMascotas(); break;
                    case "7": ActualizarMascota(); break;
                    case "8": EliminarMascota(); break;
                    case "9": CrearInsumo(); break;
                    case "10": ListarInsumos(); break;
                    case "11": ActualizarInsumo(); break;
                    case "12": EliminarInsumo(); break;
                    case "13": CrearConsulta(); break;
                    case "14": ListarConsultas(); break;
                    case "15": ActualizarConsulta(); break;
                    case "16": EliminarConsulta(); break;
                    case "17": DemostrarPolimorfismo(); break;
                    case "0": _continuar = false; break;
                    default: Console.WriteLine("Seleccione una opcion valida del menu."); break;
                }
            }
            catch (Exception error)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"Operacion controlada con try-catch: {TraducirError(error)}");
                Console.ResetColor();
            }

            if (_continuar) Pausar();
        }
    }

    private static void MostrarMenu()
    {
        if (!Console.IsOutputRedirected) Console.Clear();
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine("====================================================");
        Console.WriteLine("       Sistema Veterinario Version 2.0              ");
        Console.WriteLine("====================================================");
        Console.ResetColor();
        Console.WriteLine("PROPIETARIOS");
        Console.WriteLine(" 1. Crear propietario   2. Listar propietarios");
        Console.WriteLine(" 3. Actualizar propietario   4. Eliminar propietario");
        Console.WriteLine("MASCOTAS");
        Console.WriteLine(" 5. Crear mascota   6. Listar mascotas");
        Console.WriteLine(" 7. Actualizar mascota   8. Eliminar mascota");
        Console.WriteLine("INVENTARIO");
        Console.WriteLine(" 9. Crear insumo   10. Listar insumos");
        Console.WriteLine("11. Actualizar insumo   12. Eliminar insumo");
        Console.WriteLine("CONSULTAS");
        Console.WriteLine("13. Registrar consulta   14. Listar consultas");
        Console.WriteLine("15. Actualizar consulta   16. Eliminar consulta");
        Console.WriteLine("POO");
        Console.WriteLine("17. Demostrar polimorfismo");
        Console.WriteLine(" 0. Salir");
        Console.Write("Opcion: ");
    }

    private void CrearPropietario()
    {
        _repositorio.CrearPropietario(LeerPropietario());
        Console.WriteLine("Propietario creado correctamente.");
    }

    private void ListarPropietarios() => MostrarTabla(_repositorio.ObtenerPropietarios(), "PROPIETARIOS");

    private void ActualizarPropietario()
    {
        _repositorio.ActualizarPropietario(LeerId("Id del propietario a actualizar"), LeerPropietario());
        Console.WriteLine("Propietario actualizado correctamente (si el Id existe).");
    }

    private void EliminarPropietario()
    {
        _repositorio.EliminarPropietario(LeerId("Id del propietario a eliminar"));
        Console.WriteLine("Propietario eliminado. Sus mascotas y consultas relacionadas tambien se eliminan.");
    }

    private void CrearMascota()
    {
        _repositorio.CrearMascota(LeerMascota());
        Console.WriteLine("Mascota creada correctamente.");
    }

    private void ListarMascotas() => MostrarTabla(_repositorio.ObtenerMascotas(), "MASCOTAS");

    private void ActualizarMascota()
    {
        var id = LeerId("Id de la mascota a actualizar");
        _repositorio.ActualizarMascota(id, LeerMascota());
        Console.WriteLine("Mascota actualizada correctamente (si el Id existe).");
    }

    private void EliminarMascota()
    {
        _repositorio.EliminarMascota(LeerId("Id de la mascota a eliminar"));
        Console.WriteLine("Mascota eliminada. Sus consultas relacionadas tambien se eliminan.");
    }

    private void CrearInsumo()
    {
        _repositorio.CrearInsumo(LeerInsumo());
        Console.WriteLine("Insumo creado correctamente.");
    }

    private void ListarInsumos() => MostrarTabla(_repositorio.ObtenerInsumos(), "INVENTARIO DE INSUMOS");

    private void ActualizarInsumo()
    {
        _repositorio.ActualizarInsumo(LeerId("Id del insumo a actualizar"), LeerInsumo());
        Console.WriteLine("Insumo actualizado correctamente (si el Id existe).");
    }

    private void EliminarInsumo()
    {
        _repositorio.EliminarInsumo(LeerId("Id del insumo a eliminar"));
        Console.WriteLine("Insumo eliminado.");
    }

    private void CrearConsulta()
    {
        var consulta = LeerConsulta();
        var alerta = _repositorio.CrearConsulta(consulta);
        Console.WriteLine("Consulta registrada correctamente.");
        Console.WriteLine($"Estado clinico: {consulta.ObtenerEstadoClinico()}");
        MostrarAlerta(alerta);
    }

    private void ListarConsultas() => MostrarConsultas(_repositorio.ObtenerConsultas());

    private void ActualizarConsulta()
    {
        var id = LeerId("Id de la consulta a actualizar");
        var consulta = LeerConsulta();
        var alerta = _repositorio.ActualizarConsulta(id, consulta);
        Console.WriteLine("Consulta actualizada correctamente (si el Id existe).");
        MostrarAlerta(alerta);
    }

    private void EliminarConsulta()
    {
        _repositorio.EliminarConsulta(LeerId("Id de la consulta a eliminar"));
        Console.WriteLine("Consulta eliminada.");
    }

    private void DemostrarPolimorfismo()
    {
        SerVivo serVivo = new Mascota("EJEMPLO", "Luna", "Perro", 1);
        Console.WriteLine("Una referencia de tipo SerVivo contiene un objeto Mascota:");
        serVivo.MostrarInformacion();
    }

    private static Propietario LeerPropietario() => new(
        LeerTexto("Codigo"), LeerTexto("Nombre completo"), LeerTexto("Telefono"), LeerTexto("Correo electronico"));

    private Mascota LeerMascota()
    {
        MostrarTabla(_repositorio.ObtenerPropietariosParaSeleccion(), "PROPIETARIOS DISPONIBLES");
        return new Mascota(LeerTexto("Codigo"), LeerTexto("Nombre"), LeerTexto("Especie"), LeerId("Id del propietario"));
    }

    private static Insumo LeerInsumo() => new(
        LeerTexto("Codigo"), LeerTexto("Nombre"), LeerDecimal("Precio unitario (Q)"), LeerEntero("Stock"));

    private ConsultaVeterinaria LeerConsulta()
    {
        MostrarTabla(_repositorio.ObtenerMascotasParaSeleccion(), "MASCOTAS DISPONIBLES");
        var consulta = new ConsultaVeterinaria(
            LeerId("Id de la mascota"), LeerTexto("Servicio"), LeerTexto("Veterinario"), DateTime.Now);
        consulta.RegistrarDatos(
            LeerDecimal("Peso (kg)"), LeerDecimal("Temperatura (C)"), LeerDecimal("Costo del servicio (Q)"));

        if (LeerSiNo("Se utilizo algun insumo? (S/N)"))
        {
            MostrarTabla(_repositorio.ObtenerInsumos(), "INSUMOS DISPONIBLES");
            var insumo = _repositorio.ObtenerInsumo(LeerId("Id del insumo"))
                ?? throw new ArgumentException("El insumo seleccionado no existe.");
            consulta.AsignarInsumo(insumo, LeerEntero("Cantidad"));
        }

        return consulta;
    }

    private static int LeerId(string mensaje) => LeerEntero(mensaje);

    private static int LeerEntero(string mensaje)
    {
        Console.Write($"{mensaje}: ");
        if (!int.TryParse(Console.ReadLine(), out var valor))
            throw new ArgumentException("Ingrese un numero entero valido.");
        return valor;
    }

    private static double LeerDecimal(string mensaje)
    {
        Console.Write($"{mensaje}: ");
        var texto = (Console.ReadLine() ?? string.Empty).Trim().Replace(',', '.');
        if (!double.TryParse(texto, NumberStyles.Float, CultureInfo.InvariantCulture, out var valor))
            throw new ArgumentException("Ingrese un numero valido (ejemplo: 38.5).");
        return valor;
    }

    private static string LeerTexto(string mensaje)
    {
        Console.Write($"{mensaje}: ");
        var valor = Console.ReadLine();
        return string.IsNullOrWhiteSpace(valor)
            ? throw new ArgumentException($"El campo '{mensaje}' no puede quedar vacio.")
            : valor.Trim();
    }

    private static bool LeerSiNo(string mensaje)
    {
        Console.Write($"{mensaje}: ");
        return string.Equals(Console.ReadLine()?.Trim(), "S", StringComparison.OrdinalIgnoreCase);
    }

    private static void MostrarAlerta(string alerta)
    {
        if (alerta is null) return;
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine(alerta);
        Console.ResetColor();
    }

    private static void MostrarTabla(DataTable tabla, string titulo)
    {
        Console.WriteLine($"\n=== {titulo} ===");
        if (tabla.Rows.Count == 0)
        {
            Console.WriteLine("No hay registros.");
            return;
        }

        foreach (DataColumn columna in tabla.Columns)
            Console.Write($"{columna.ColumnName,-24}");
        Console.WriteLine();
        foreach (DataRow fila in tabla.Rows)
        {
            foreach (var valor in fila.ItemArray)
                Console.Write($"{valor,-24}");
            Console.WriteLine();
        }
    }

    private static void MostrarConsultas(DataTable tabla)
    {
        Console.WriteLine("\n=== CONSULTAS ===");
        if (tabla.Rows.Count == 0)
        {
            Console.WriteLine("No hay registros.");
            return;
        }

        foreach (DataRow fila in tabla.Rows)
        {
            var temperatura = Convert.ToDouble(fila["Temperatura"]);
            var insumo = fila["Insumo"] is DBNull ? "sin insumo" : $"{fila["Insumo"]} x{fila["CantidadInsumo"]}";
            var fecha = DateTime.Parse(fila["Fecha"].ToString(), CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind);

            Console.WriteLine($"[{fila["Id"]}] {fila["Mascota"]} ({fila["CodigoMascota"]}) | {fila["Servicio"]} | Vet: {fila["Veterinario"]} | {fecha:dd/MM/yyyy HH:mm}");
            Console.WriteLine($"     Peso: {Convert.ToDouble(fila["Peso"]):F1} kg | Temperatura: {temperatura:F1} C | Estado: {EstadoPorTemperatura(temperatura)}");
            Console.WriteLine($"     Insumo: {insumo} | Costo: Q{Convert.ToDouble(fila["Costo"]):F2} | Total: Q{Convert.ToDouble(fila["Total"]):F2}");
        }
    }

    private static string EstadoPorTemperatura(double temperatura)
    {
        if (temperatura < 38.0)
            return "Temperatura baja";
        if (temperatura > 39.2)
            return "Temperatura alta";
        return "Temperatura normal";
    }

    private static string TraducirError(Exception error)
    {
        var mensaje = error.Message;
        if (mensaje.Contains("UNIQUE constraint failed"))
            return "Ya existe un registro con ese codigo o correo.";
        if (mensaje.Contains("FOREIGN KEY constraint failed"))
            return "La operacion no es posible: el Id indicado no existe o el registro esta en uso en otra tabla.";
        if (mensaje.Contains("CHECK constraint failed"))
            return "Uno de los valores no cumple las reglas de la base de datos.";
        return mensaje;
    }

    private static void Pausar()
    {
        Console.WriteLine("\nPresione Enter para volver al menu...");
        Console.ReadLine();
    }
}