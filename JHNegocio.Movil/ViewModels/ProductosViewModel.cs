using System.Collections.ObjectModel;
using System.Windows.Input;
using JHNegocio.Movil.Models;
using JHNegocio.Movil.Services;
using JHNegocio.Movil.Views;

namespace JHNegocio.Movil.ViewModels;

public class ProductosViewModel : BaseViewModel
{
    private readonly IProductoService _servicio;

    private string _textoBusqueda = string.Empty;
    private bool _estaRefrescando;
    private string _estadoConexion = "Conectando...";
    private Color _colorEstado = Colors.Orange;
    private int _totalProductos;
    private decimal _valorInventario;
    private int _stockBajo;

    public ProductosViewModel(IProductoService servicio)
    {
        _servicio = servicio;
        Titulo = "Inventario";

        CargarCommand = new Command(async () => await CargarAsync());
        BuscarCommand = new Command(async () => await CargarAsync());
        AgregarCommand = new Command(async () => await Shell.Current.GoToAsync(nameof(NuevoProductoPage)));
        EliminarCommand = new Command<Producto>(async p => await EliminarAsync(p));
        CambiarServidorCommand = new Command(async () => await CambiarServidorAsync());
        MultimediaCommand = new Command(async () => await Shell.Current.GoToAsync(nameof(MultimediaPage)));
    }

    public ObservableCollection<Producto> Productos { get; } = new();

    public ICommand CargarCommand { get; }
    public ICommand BuscarCommand { get; }
    public ICommand AgregarCommand { get; }
    public ICommand EliminarCommand { get; }
    public ICommand CambiarServidorCommand { get; }
    public ICommand MultimediaCommand { get; }

    public string TextoBusqueda { get => _textoBusqueda; set => SetProperty(ref _textoBusqueda, value); }
    public bool EstaRefrescando { get => _estaRefrescando; set => SetProperty(ref _estaRefrescando, value); }
    public string EstadoConexion { get => _estadoConexion; set => SetProperty(ref _estadoConexion, value); }
    public Color ColorEstado { get => _colorEstado; set => SetProperty(ref _colorEstado, value); }
    public int TotalProductos { get => _totalProductos; set => SetProperty(ref _totalProductos, value); }
    public decimal ValorInventario { get => _valorInventario; set => SetProperty(ref _valorInventario, value); }
    public int StockBajo { get => _stockBajo; set => SetProperty(ref _stockBajo, value); }

    public string UrlApi => $"{ApiConfig.BaseUrl}api/productos";

    /// <summary>Llama a GET /api/productos y GET /api/productos/resumen.</summary>
    public async Task CargarAsync()
    {
        if (EstaOcupado) return;

        try
        {
            EstaOcupado = true;

            var productos = await _servicio.ObtenerTodosAsync(TextoBusqueda);
            var resumen = await _servicio.ObtenerResumenAsync();

            Productos.Clear();
            foreach (var p in productos)
                Productos.Add(p);

            if (resumen is not null)
            {
                TotalProductos = resumen.TotalProductos;
                ValorInventario = resumen.ValorInventario;
                StockBajo = resumen.ProductosStockBajo;
            }

            EstadoConexion = $"Conectado · {DateTime.Now:dd/MM/yyyy hh:mm tt}";
            ColorEstado = Color.FromArgb("#00E676");
        }
        catch (Exception ex)
        {
            EstadoConexion = "Sin conexión con la API";
            ColorEstado = Color.FromArgb("#FF5252");
            await Shell.Current.DisplayAlert(
                "Error de conexión",
                $"No se pudo consumir el servicio REST.\n\n{ex.Message}\n\nVerifica que JHNegocio.Api esté ejecutándose.",
                "OK");
        }
        finally
        {
            EstaOcupado = false;
            EstaRefrescando = false;
        }
    }

    /// <summary>Permite escribir la URL de la API (por ejemplo, la de Codespaces).</summary>
    private async Task CambiarServidorAsync()
    {
        var url = await Shell.Current.DisplayPromptAsync(
            "Servidor de la API",
            "Escribe la URL del servicio REST.\nEj.: https://mi-codespace-5080.app.github.dev",
            "Guardar", "Cancelar",
            initialValue: ApiConfig.BaseUrl,
            keyboard: Keyboard.Url);

        if (string.IsNullOrWhiteSpace(url)) return;

        ApiConfig.BaseUrl = url;
        OnPropertyChanged(nameof(UrlApi));
        await CargarAsync();
    }

    /// <summary>Llama a DELETE /api/productos/{id}.</summary>
    private async Task EliminarAsync(Producto? producto)
    {
        if (producto is null) return;

        bool confirmar = await Shell.Current.DisplayAlert(
            "Eliminar producto", $"¿Eliminar \"{producto.Nombre}\"?", "Eliminar", "Cancelar");
        if (!confirmar) return;

        try
        {
            if (await _servicio.EliminarAsync(producto.Id))
            {
                Productos.Remove(producto);
                await CargarAsync();
            }
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlert("Error", ex.Message, "OK");
        }
    }
}
