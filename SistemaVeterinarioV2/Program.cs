
using SistemaVeterinario.Persistencia;
using SistemaVeterinario.Aplicacion;

namespace SistemaVeterinario;

internal static class Program
{
    private static void Main()
    {
        try
        {
            var inicializador = new InicializadorBaseDatos();
            inicializador.Inicializar();

            var repositorio = new RepositorioVeterinario(
                inicializador.CadenaConexion
            );

            Console.WriteLine($"Base de datos lista en: {inicializador.RutaBaseDatos}");
            Console.WriteLine($"Propietarios registrados: {repositorio.ObtenerPropietarios().Rows.Count}");

            var menu = new MenuVeterinario(repositorio);
            menu.Ejecutar();
        }
        catch (Exception error)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"No fue posible iniciar el sistema: {error.Message}");
            Console.ResetColor();
        }
    }
}
