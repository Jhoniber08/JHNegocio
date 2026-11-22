using System.Globalization;
using System.Windows.Input;
using JHNegocio.Movil.Models;
using JHNegocio.Movil.Services;

namespace JHNegocio.Movil.ViewModels;

public class NuevoProductoViewModel : BaseViewModel
{
    private readonly IProductoService _servicio;

    private string _codigo = string.Empty;
    private string _nombre = string.Empty;
    private string _categoria = string.Empty;
    private string _precio = string.Empty;
    private string _cantidad = string.Empty;

    public NuevoProductoViewModel(IProductoService servicio)
    {
        _servicio = servicio;
        Titulo = "Nuevo producto";
        GuardarCommand = new Command(async () => await GuardarAsync());
        CancelarCommand = new Command(async () => await Shell.Current.GoToAsync(".."));
    }

    public ICommand GuardarCommand { get; }
    public ICommand CancelarCommand { get; }

    public string Codigo { get => _codigo; set => SetProperty(ref _codigo, value); }
    public string Nombre { get => _nombre; set => SetProperty(ref _nombre, value); }
    public string Categoria { get => _categoria; set => SetProperty(ref _categoria, value); }
    public string Precio { get => _precio; set => SetProperty(ref _precio, value); }
    public string Cantidad { get => _cantidad; set => SetProperty(ref _cantidad, value); }

    /// <summary>Valida y envía POST /api/productos.</summary>
    private async Task GuardarAsync()
    {
        if (EstaOcupado) return;

        if (string.IsNullOrWhiteSpace(Codigo) || string.IsNullOrWhiteSpace(Nombre))
        {
            await Shell.Current.DisplayAlert("Datos incompletos", "El código y el nombre son obligatorios.", "OK");
            return;
        }

        var textoPrecio = (Precio ?? string.Empty).Replace(',', '.');
        if (!decimal.TryParse(textoPrecio, NumberStyles.Number, CultureInfo.InvariantCulture, out var precio) || precio <= 0)
        {
            await Shell.Current.DisplayAlert("Precio inválido", "Escribe un precio mayor que 0.", "OK");
            return;
        }

        if (!int.TryParse(Cantidad, out var cantidad) || cantidad < 0)
        {
            await Shell.Current.DisplayAlert("Cantidad inválida", "Escribe una cantidad entera (0 o más).", "OK");
            return;
        }

        try
        {
            EstaOcupado = true;

            var creado = await _servicio.CrearAsync(new ProductoDto
            {
                Codigo = Codigo.Trim(),
                Nombre = Nombre.Trim(),
                Categoria = string.IsNullOrWhiteSpace(Categoria) ? "General" : Categoria.Trim(),
                Precio = precio,
                Cantidad = cantidad
            });

            await Shell.Current.DisplayAlert(
                "Producto guardado",
                $"La API creó el producto con Id {creado?.Id}.\n{creado?.Nombre}",
                "OK");

            await Shell.Current.GoToAsync("..");
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlert("Error al guardar", ex.Message, "OK");
        }
        finally
        {
            EstaOcupado = false;
        }
    }
}
