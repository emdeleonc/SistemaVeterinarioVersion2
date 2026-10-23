namespace SistemaVeterinario.Dominio;

public sealed class Propietario
{
    public Propietario(string codigo, string nombre, string telefono, string correo)
    {
        Codigo = ValidarTexto(codigo, "El codigo es obligatorio.");
        Nombre = ValidarTexto(nombre, "El nombre es obligatorio.");
        Telefono = ValidarTexto(telefono, "El telefono es obligatorio.");
        Correo = ValidarTexto(correo, "El correo es obligatorio.");
        if (!Correo.Contains('@')) throw new ArgumentException("El correo debe contener @.");
    }

    public int Id { get; set; }
    public string Codigo { get; }
    public string Nombre { get; }
    public string Telefono { get; }
    public string Correo { get; }

    public string ObtenerInformacion() => $"{Codigo} - {Nombre} | Tel: {Telefono} | {Correo}";

    private static string ValidarTexto(string valor, string mensaje) =>
        string.IsNullOrWhiteSpace(valor) ? throw new ArgumentException(mensaje) : valor.Trim();
}
