namespace JHNegocio.Movil.Services;

/// <summary>
/// Dirección de la API REST. Se guarda en Preferences y se puede cambiar
/// desde la app (botón "Servidor"), por ejemplo a la URL pública de Codespaces:
///   https://NOMBRE-5080.app.github.dev/
/// Valores por defecto:
/// - Emulador Android: 10.0.2.2 apunta al "localhost" de la PC.
/// - Windows: localhost directo.
/// </summary>
public static class ApiConfig
{
    public const int Puerto = 5080;
    private const string Clave = "api_url";

    public static string UrlPorDefecto =>
        DeviceInfo.Platform == DevicePlatform.Android
            ? $"http://10.0.2.2:{Puerto}/"
            : $"http://localhost:{Puerto}/";

    public static string BaseUrl
    {
        get => Preferences.Default.Get(Clave, UrlPorDefecto);
        set => Preferences.Default.Set(Clave, Normalizar(value));
    }

    private static string Normalizar(string url)
    {
        url = (url ?? string.Empty).Trim();
        if (string.IsNullOrEmpty(url)) return UrlPorDefecto;
        if (!url.StartsWith("http://", StringComparison.OrdinalIgnoreCase) &&
            !url.StartsWith("https://", StringComparison.OrdinalIgnoreCase))
            url = "https://" + url;
        return url.EndsWith('/') ? url : url + "/";
    }
}
