using JHNegocio.Movil.Services;
using JHNegocio.Movil.ViewModels;
using JHNegocio.Movil.Views;

namespace JHNegocio.Movil;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder.UseMauiApp<App>();

        // HttpClient apuntando a la API REST de JH Negocio
        // (la URL se toma de ApiConfig en cada llamada)
        builder.Services.AddSingleton(_ => new HttpClient
        {
            Timeout = TimeSpan.FromSeconds(20)
        });

        // Servicio que consume la API
        builder.Services.AddSingleton<IProductoService, ProductoService>();

        // ViewModels y páginas (inyección de dependencias)
        builder.Services.AddTransient<ProductosViewModel>();
        builder.Services.AddTransient<ProductosPage>();
        builder.Services.AddTransient<NuevoProductoViewModel>();
        builder.Services.AddTransient<NuevoProductoPage>();

        return builder.Build();
    }
}
