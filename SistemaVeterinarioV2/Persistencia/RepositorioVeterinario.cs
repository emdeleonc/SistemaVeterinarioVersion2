using System.Data;
using Microsoft.Data.Sqlite;
using SistemaVeterinario.Dominio;

namespace SistemaVeterinario.Persistencia;

internal sealed class RepositorioVeterinario
{
    private readonly string _cadenaConexion;

    public RepositorioVeterinario(string cadenaConexion)
    {
        _cadenaConexion = cadenaConexion;
    }

    // CRUD de propietarios
    public DataTable ObtenerPropietarios() => Consultar(
        "SELECT Id, Codigo, Nombre, Telefono, Correo FROM Propietarios ORDER BY Nombre;");

    public DataTable ObtenerPropietariosParaSeleccion() => Consultar(
        "SELECT Id, Codigo || ' - ' || Nombre AS Propietario FROM Propietarios ORDER BY Nombre;");

    public void CrearPropietario(Propietario propietario) => Ejecutar(
        "INSERT INTO Propietarios (Codigo, Nombre, Telefono, Correo) VALUES ($codigo, $nombre, $telefono, $correo);",
        ("$codigo", propietario.Codigo), ("$nombre", propietario.Nombre),
        ("$telefono", propietario.Telefono), ("$correo", propietario.Correo));

    public void ActualizarPropietario(int id, Propietario propietario) => Ejecutar(
        "UPDATE Propietarios SET Codigo = $codigo, Nombre = $nombre, Telefono = $telefono, Correo = $correo WHERE Id = $id;",
        ("$codigo", propietario.Codigo), ("$nombre", propietario.Nombre),
        ("$telefono", propietario.Telefono), ("$correo", propietario.Correo), ("$id", id));

    public void EliminarPropietario(int id) => Ejecutar(
        "DELETE FROM Propietarios WHERE Id = $id;", ("$id", id));

    // CRUD de mascotas
    public DataTable ObtenerMascotas() => Consultar(@"SELECT m.Id, m.Codigo, m.Nombre, m.Especie,
        m.PropietarioId, p.Nombre AS Propietario
        FROM Mascotas m
        INNER JOIN Propietarios p ON p.Id = m.PropietarioId
        ORDER BY m.Nombre;");

    public DataTable ObtenerMascotasParaSeleccion() => Consultar(
        "SELECT Id, Codigo || ' - ' || Nombre AS Mascota FROM Mascotas ORDER BY Nombre;");

    public void CrearMascota(Mascota mascota) => Ejecutar(
        "INSERT INTO Mascotas (Codigo, Nombre, Especie, PropietarioId) VALUES ($codigo, $nombre, $especie, $propietario);",
        ("$codigo", mascota.Codigo), ("$nombre", mascota.Nombre),
        ("$especie", mascota.Especie), ("$propietario", mascota.PropietarioId));

    public void ActualizarMascota(int id, Mascota mascota) => Ejecutar(
        "UPDATE Mascotas SET Codigo = $codigo, Nombre = $nombre, Especie = $especie, PropietarioId = $propietario WHERE Id = $id;",
        ("$codigo", mascota.Codigo), ("$nombre", mascota.Nombre),
        ("$especie", mascota.Especie), ("$propietario", mascota.PropietarioId), ("$id", id));

    public void EliminarMascota(int id) => Ejecutar(
        "DELETE FROM Mascotas WHERE Id = $id;", ("$id", id));

    // CRUD de insumos (inventario)
    public DataTable ObtenerInsumos() => Consultar(
        "SELECT Id, Codigo, Nombre, Precio, Stock FROM Insumos ORDER BY Nombre;");

    public DataTable ObtenerInsumosParaSeleccion() => Consultar(
        "SELECT Id, Codigo || ' - ' || Nombre AS Insumo FROM Insumos ORDER BY Nombre;");

    public Insumo ObtenerInsumo(int id)
    {
        using var conexion = AbrirConexion();
        using var comando = conexion.CreateCommand();
        comando.CommandText = "SELECT Id, Codigo, Nombre, Precio, Stock FROM Insumos WHERE Id = $id;";
        comando.Parameters.AddWithValue("$id", id);
        using var lector = comando.ExecuteReader();
        if (!lector.Read()) return null;
        return new Insumo(lector.GetString(1), lector.GetString(2), lector.GetDouble(3), lector.GetInt32(4))
        {
            Id = lector.GetInt32(0)
        };
    }

    public void CrearInsumo(Insumo insumo) => Ejecutar(
        "INSERT INTO Insumos (Codigo, Nombre, Precio, Stock) VALUES ($codigo, $nombre, $precio, $stock);",
        ("$codigo", insumo.Codigo), ("$nombre", insumo.Nombre),
        ("$precio", insumo.Precio), ("$stock", insumo.Stock));

    public void ActualizarInsumo(int id, Insumo insumo) => Ejecutar(
        "UPDATE Insumos SET Codigo = $codigo, Nombre = $nombre, Precio = $precio, Stock = $stock WHERE Id = $id;",
        ("$codigo", insumo.Codigo), ("$nombre", insumo.Nombre),
        ("$precio", insumo.Precio), ("$stock", insumo.Stock), ("$id", id));

    public void EliminarInsumo(int id) => Ejecutar(
        "DELETE FROM Insumos WHERE Id = $id;", ("$id", id));

    // CRUD de consultas
    // El Total usa la misma formula que ConsultaVeterinaria.CalcularTotal():
    // costo del servicio + (precio del insumo x cantidad), si hubo insumo.
    public DataTable ObtenerConsultas() => Consultar(@"SELECT c.Id, c.MascotaId, c.InsumoId,
        m.Codigo AS CodigoMascota, m.Nombre AS Mascota,
        c.Servicio, c.Veterinario, c.Peso, c.Temperatura, c.Costo,
        i.Nombre AS Insumo, c.CantidadInsumo,
        c.Costo + COALESCE(i.Precio * c.CantidadInsumo, 0) AS Total, c.Fecha
        FROM Consultas c
        INNER JOIN Mascotas m ON m.Id = c.MascotaId
        LEFT JOIN Insumos i ON i.Id = c.InsumoId
        ORDER BY c.Fecha DESC, c.Id DESC;");

    // Devuelve la alerta al administrador (RN-03) o null si no hubo ninguna.
    public string CrearConsulta(ConsultaVeterinaria consulta)
    {
        ValidarListaParaGuardar(consulta);
        var (insumoId, cantidad, alerta) = ResolverInsumo(consulta);

        Ejecutar(@"INSERT INTO Consultas (MascotaId, Servicio, Veterinario, Peso, Temperatura, Costo,
                InsumoId, CantidadInsumo, Fecha)
            VALUES ($mascota, $servicio, $veterinario, $peso, $temperatura, $costo, $insumo, $cantidad, $fecha);",
            ("$mascota", consulta.MascotaId), ("$servicio", consulta.Servicio),
            ("$veterinario", consulta.Veterinario), ("$peso", consulta.Peso),
            ("$temperatura", consulta.Temperatura), ("$costo", consulta.Costo),
            ("$insumo", insumoId), ("$cantidad", cantidad),
            ("$fecha", consulta.Fecha.ToString("O")));

        return alerta;
    }

    public string ActualizarConsulta(int id, ConsultaVeterinaria consulta)
    {
        ValidarListaParaGuardar(consulta);
        var (insumoId, cantidad, alerta) = ResolverInsumo(consulta);

        Ejecutar(@"UPDATE Consultas SET MascotaId = $mascota, Servicio = $servicio, Veterinario = $veterinario,
                Peso = $peso, Temperatura = $temperatura, Costo = $costo,
                InsumoId = $insumo, CantidadInsumo = $cantidad, Fecha = $fecha
            WHERE Id = $id;",
            ("$mascota", consulta.MascotaId), ("$servicio", consulta.Servicio),
            ("$veterinario", consulta.Veterinario), ("$peso", consulta.Peso),
            ("$temperatura", consulta.Temperatura), ("$costo", consulta.Costo),
            ("$insumo", insumoId), ("$cantidad", cantidad),
            ("$fecha", consulta.Fecha.ToString("O")), ("$id", id));

        return alerta;
    }

    public void EliminarConsulta(int id) => Ejecutar(
        "DELETE FROM Consultas WHERE Id = $id;", ("$id", id));

    private static void ValidarListaParaGuardar(ConsultaVeterinaria consulta)
    {
        if (!consulta.DatosRegistrados)
            throw new InvalidOperationException("Debe registrar peso, temperatura y costo antes de guardar la consulta.");
    }

    // RN-03: se verifica el stock del insumo solicitado.
    // - Hay stock: la consulta se guarda con el insumo (el stock NO se descuenta, solo se demuestra la regla).
    // - No hay stock: se genera la alerta al administrador y la consulta se guarda igual, sin el insumo.
    private (int? InsumoId, int Cantidad, string Alerta) ResolverInsumo(ConsultaVeterinaria consulta)
    {
        if (consulta.Insumo is null) return (null, 0, null);

        var insumo = ObtenerInsumo(consulta.Insumo.Id)
            ?? throw new InvalidOperationException("El insumo seleccionado no existe.");

        if (insumo.HayStock(consulta.CantidadInsumo))
            return (insumo.Id, consulta.CantidadInsumo, null);

        var alerta = $"ALERTA AL ADMINISTRADOR: stock insuficiente de '{insumo.Nombre}' " +
                     $"(disponible: {insumo.Stock}, solicitado: {consulta.CantidadInsumo}). " +
                     "La consulta se guardo sin el insumo (RN-03).";
        return (null, 0, alerta);
    }

    private DataTable Consultar(string sql)
    {
        using var conexion = AbrirConexion();
        using var comando = conexion.CreateCommand();
        comando.CommandText = sql;
        using var lector = comando.ExecuteReader();
        var tabla = new DataTable();
        tabla.Load(lector);
        return tabla;
    }

    private void Ejecutar(string sql, params (string Nombre, object Valor)[] parametros)
    {
        using var conexion = AbrirConexion();
        using var comando = conexion.CreateCommand();
        comando.CommandText = sql;
        foreach (var (nombre, valor) in parametros)
            comando.Parameters.AddWithValue(nombre, valor ?? DBNull.Value);
        comando.ExecuteNonQuery();
    }

    private SqliteConnection AbrirConexion()
    {
        var conexion = new SqliteConnection(_cadenaConexion);
        conexion.Open();
        return conexion;
    }
}
