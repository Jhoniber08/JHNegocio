using JHNegocio.Movil.ViewModels;

namespace JHNegocio.Movil.Views;

public partial class NuevoProductoPage : ContentPage
{
    public NuevoProductoPage(NuevoProductoViewModel vm)
    {
        InitializeComponent();
        BindingContext = vm;
    }
}
