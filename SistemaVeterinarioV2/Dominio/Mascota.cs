namespace SistemaVeterinario.Dominio;

public class Mascota : SerVivo
{
    public Mascota(string codigo, string nombre, string especie, int propietarioId)
        : base(codigo, nombre)
    {
        Especie = ValidarTexto(especie, "La especie es obligatoria.");
        // RN-01: toda mascota pertenece a un propietario registrado.
        if (propietarioId <= 0) throw new ArgumentException("Debe seleccionar un propietario.");
        PropietarioId = propietarioId;
    }

    public int Id { get; set; }
    public string Especie { get; }
    public int PropietarioId { get; }

    public override void MostrarInformacion()
    {
        Console.WriteLine($"{Codigo} - {Nombre} - {Especie} - Propietario Id: {PropietarioId}");
    }
}
