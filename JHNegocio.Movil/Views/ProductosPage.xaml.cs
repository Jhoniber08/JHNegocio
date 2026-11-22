using JHNegocio.Movil.ViewModels;

namespace JHNegocio.Movil.Views;

public partial class ProductosPage : ContentPage
{
    private readonly ProductosViewModel _vm;

    public ProductosPage(ProductosViewModel vm)
    {
        InitializeComponent();
        BindingContext = _vm = vm;
    }

    // Cada vez que se muestra la pantalla, consulta la API
    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await _vm.CargarAsync();
    }
}
