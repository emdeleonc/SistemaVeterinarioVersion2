namespace SistemaVeterinario.Dominio;

public class ConsultaVeterinaria
{
    public ConsultaVeterinaria(int mascotaId, string servicio, string veterinario, DateTime fecha)
    {
        if (mascotaId <= 0) throw new ArgumentException("Debe seleccionar una mascota.");
        if (string.IsNullOrWhiteSpace(servicio)) throw new ArgumentException("El servicio es obligatorio.");
        if (string.IsNullOrWhiteSpace(veterinario)) throw new ArgumentException("El veterinario es obligatorio.");
        MascotaId = mascotaId;
        Servicio = servicio.Trim();
        Veterinario = veterinario.Trim();
        Fecha = fecha;
    }

    public int Id { get; set; }
    public int MascotaId { get; }
    public string Servicio { get; }
    public string Veterinario { get; }
    public DateTime Fecha { get; }
    public double Peso { get; private set; }
    public double Temperatura { get; private set; }
    public double Costo { get; private set; }
    public Insumo Insumo { get; private set; }
    public int CantidadInsumo { get; private set; }

    
    public bool DatosRegistrados => Peso > 0 && Temperatura > 0 && Costo > 0;

    public void RegistrarDatos(double peso, double temperatura, double costo)
    {
        ValidarPeso(peso);
        ValidarTemperatura(temperatura);
        ValidarCosto(costo);
        Peso = peso;
        Temperatura = temperatura;
        Costo = costo;
    }

    
    public void AsignarInsumo(Insumo insumo, int cantidad)
    {
        if (insumo is null) throw new ArgumentException("El insumo es obligatorio.");
        if (cantidad <= 0) throw new ArgumentException("La cantidad de insumo debe ser mayor que 0.");
        Insumo = insumo;
        CantidadInsumo = cantidad;
    }

    
    public double CalcularTotal()
    {
        double totalInsumo = Insumo is null ? 0 : Insumo.Precio * CantidadInsumo;
        return Costo + totalInsumo;
    }

    public string ObtenerEstadoClinico()
    {
        return Temperatura >= 39.5 ? "Requiere observacion" : "Estable";
    }

    public string ObtenerResumen(string nombreMascota)
    {
        return $"{nombreMascota} | {Servicio} | Peso: {Peso:F1} kg | Temperatura: {Temperatura:F1} C | Total: Q{CalcularTotal():F2} | Estado: {ObtenerEstadoClinico()}";
    }

    private static void ValidarPeso(double peso)
    {
        if (peso <= 0) throw new ArgumentException("El peso debe ser mayor que 0.");
    }

    private static void ValidarTemperatura(double temperatura)
    {
        if (temperatura < 35 || temperatura > 43)
            throw new ArgumentException("La temperatura debe estar entre 35 y 43 grados.");
    }

    private static void ValidarCosto(double costo)
    {
        if (costo <= 0) throw new ArgumentException("El costo debe ser mayor que 0.");
    }
}
