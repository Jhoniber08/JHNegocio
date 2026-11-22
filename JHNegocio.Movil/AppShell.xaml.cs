using JHNegocio.Movil.Views;

namespace JHNegocio.Movil;

public partial class AppShell : Shell
{
    public AppShell()
    {
        InitializeComponent();

        // Ruta para navegar a la pantalla de nuevo producto
        Routing.RegisterRoute(nameof(NuevoProductoPage), typeof(NuevoProductoPage));
        Routing.RegisterRoute(nameof(MultimediaPage), typeof(MultimediaPage));
    }
}
