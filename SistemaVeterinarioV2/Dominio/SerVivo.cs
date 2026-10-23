namespace SistemaVeterinario.Dominio;

public abstract class SerVivo
{
    protected SerVivo(string codigo, string nombre)
    {
        Codigo = ValidarTexto(codigo, "El codigo es obligatorio.");
        Nombre = ValidarTexto(nombre, "El nombre es obligatorio.");
    }

    public string Codigo { get; }
    public string Nombre { get; }

    public abstract void MostrarInformacion();

    protected static string ValidarTexto(string valor, string mensaje) =>
        string.IsNullOrWhiteSpace(valor) ? throw new ArgumentException(mensaje) : valor.Trim();
}
