using JHNegocio.Movil.Models;

namespace JHNegocio.Movil.Services;

public interface IProductoService
{
    Task<List<Producto>> ObtenerTodosAsync(string? buscar = null, CancellationToken ct = default);
    Task<ResumenInventario?> ObtenerResumenAsync(CancellationToken ct = default);
    Task<Producto?> CrearAsync(ProductoDto dto, CancellationToken ct = default);
    Task<bool> EliminarAsync(int id, CancellationToken ct = default);
}
