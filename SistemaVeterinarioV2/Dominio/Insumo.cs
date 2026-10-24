namespace SistemaVeterinario.Dominio;

public sealed class Insumo
{
    public Insumo(string codigo, string nombre, double precio, int stock)
    {
        Codigo = ValidarTexto(codigo, "El codigo es obligatorio.");
        Nombre = ValidarTexto(nombre, "El nombre es obligatorio.");
        if (precio <= 0) throw new ArgumentException("El precio debe ser mayor que 0.");
        if (stock < 0) throw new ArgumentException("El stock no puede ser negativo.");
        Precio = precio;
        Stock = stock;
    }

    public int Id { get; set; }
    public string Codigo { get; }
    public string Nombre { get; }
    public double Precio { get; }
    public int Stock { get; }

    public bool HayStock(int cantidad) => cantidad > 0 && Stock >= cantidad;

    public string ObtenerInformacion() => $"{Codigo} - {Nombre} | Q{Precio:F2} | Stock: {Stock}";

    private static string ValidarTexto(string valor, string mensaje) =>
        string.IsNullOrWhiteSpace(valor) ? throw new ArgumentException(mensaje) : valor.Trim();
}
