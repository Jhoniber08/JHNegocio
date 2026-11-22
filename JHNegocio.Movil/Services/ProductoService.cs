using System.Net.Http.Json;
using System.Text.Json;
using JHNegocio.Movil.Models;

namespace JHNegocio.Movil.Services;

/// <summary>
/// Consume el servicio REST de JH Negocio con HttpClient.
/// GET, POST y DELETE sobre /api/productos.
/// </summary>
public class ProductoService : IProductoService
{
    private readonly HttpClient _http;
    private readonly JsonSerializerOptions _json = new() { PropertyNameCaseInsensitive = true };

    public ProductoService(HttpClient http) => _http = http;

    // Se arma en cada llamada para respetar el servidor elegido en la app
    private static string Endpoint => $"{ApiConfig.BaseUrl}api/productos";

    // GET /api/productos?buscar=...
    public async Task<List<Producto>> ObtenerTodosAsync(string? buscar = null, CancellationToken ct = default)
    {
        var url = string.IsNullOrWhiteSpace(buscar)
            ? Endpoint
            : $"{Endpoint}?buscar={Uri.EscapeDataString(buscar)}";

        var productos = await _http.GetFromJsonAsync<List<Producto>>(url, _json, ct);
        return productos ?? new List<Producto>();
    }

    // GET /api/productos/resumen
    public Task<ResumenInventario?> ObtenerResumenAsync(CancellationToken ct = default) =>
        _http.GetFromJsonAsync<ResumenInventario>($"{Endpoint}/resumen", _json, ct);

    // POST /api/productos
    public async Task<Producto?> CrearAsync(ProductoDto dto, CancellationToken ct = default)
    {
        var respuesta = await _http.PostAsJsonAsync(Endpoint, dto, ct);

        if (!respuesta.IsSuccessStatusCode)
        {
            var detalle = await respuesta.Content.ReadAsStringAsync(ct);
            throw new HttpRequestException($"La API respondió {(int)respuesta.StatusCode}: {detalle}");
        }

        return await respuesta.Content.ReadFromJsonAsync<Producto>(_json, ct);
    }

    // DELETE /api/productos/{id}
    public async Task<bool> EliminarAsync(int id, CancellationToken ct = default)
    {
        var respuesta = await _http.DeleteAsync($"{Endpoint}/{id}", ct);
        return respuesta.IsSuccessStatusCode;
    }
}
