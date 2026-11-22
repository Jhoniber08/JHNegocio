using JHNegocio.Api.Models;

namespace JHNegocio.Api.Data;

public interface IProductoRepository
{
    IEnumerable<Producto> ObtenerTodos(string? buscar = null);
    Producto? ObtenerPorId(int id);
    Producto Crear(ProductoDto dto);
    Producto? Actualizar(int id, ProductoDto dto);
    bool Eliminar(int id);
    ResumenInventario ObtenerResumen();
}

/// <summary>
/// Repositorio en memoria con datos de ejemplo.
/// Usa lock para que sea seguro con varias peticiones a la vez.
/// </summary>
public class ProductoRepository : IProductoRepository
{
    public const int LimiteStockBajo = 5;

    private readonly List<Producto> _productos = new();
    private readonly object _lock = new();
    private int _siguienteId = 1;

    public ProductoRepository()
    {
        Sembrar("COL-001", "Arroz selecto 10 lb", "Granos", 450m, 24);
        Sembrar("COL-002", "Habichuelas rojas 1 lb", "Granos", 85m, 40);
        Sembrar("COL-003", "Aceite vegetal 1 L", "Aceites", 210m, 18);
        Sembrar("COL-004", "Salami 1 lb", "Embutidos", 175m, 4);
        Sembrar("COL-005", "Café molido 1 lb", "Bebidas", 320m, 12);
        Sembrar("COL-006", "Azúcar crema 5 lb", "Endulzantes", 190m, 3);
        Sembrar("COL-007", "Leche evaporada", "Lácteos", 75m, 30);
    }

    private void Sembrar(string codigo, string nombre, string categoria, decimal precio, int cantidad) =>
        _productos.Add(new Producto
        {
            Id = _siguienteId++,
            Codigo = codigo,
            Nombre = nombre,
            Categoria = categoria,
            Precio = precio,
            Cantidad = cantidad
        });

    public IEnumerable<Producto> ObtenerTodos(string? buscar = null)
    {
        lock (_lock)
        {
            var consulta = _productos.AsEnumerable();
            if (!string.IsNullOrWhiteSpace(buscar))
            {
                consulta = consulta.Where(p =>
                    p.Nombre.Contains(buscar, StringComparison.OrdinalIgnoreCase) ||
                    p.Codigo.Contains(buscar, StringComparison.OrdinalIgnoreCase) ||
                    p.Categoria.Contains(buscar, StringComparison.OrdinalIgnoreCase));
            }
            return consulta.OrderBy(p => p.Nombre).ToList();
        }
    }

    public Producto? ObtenerPorId(int id)
    {
        lock (_lock) return _productos.FirstOrDefault(p => p.Id == id);
    }

    public Producto Crear(ProductoDto dto)
    {
        lock (_lock)
        {
            var producto = new Producto
            {
                Id = _siguienteId++,
                Codigo = dto.Codigo.Trim().ToUpperInvariant(),
                Nombre = dto.Nombre.Trim(),
                Categoria = string.IsNullOrWhiteSpace(dto.Categoria) ? "General" : dto.Categoria.Trim(),
                Precio = dto.Precio,
                Cantidad = dto.Cantidad,
                FechaRegistro = DateTime.Now
            };
            _productos.Add(producto);
            return producto;
        }
    }

    public Producto? Actualizar(int id, ProductoDto dto)
    {
        lock (_lock)
        {
            var p = _productos.FirstOrDefault(x => x.Id == id);
            if (p is null) return null;

            p.Codigo = dto.Codigo.Trim().ToUpperInvariant();
            p.Nombre = dto.Nombre.Trim();
            p.Categoria = string.IsNullOrWhiteSpace(dto.Categoria) ? "General" : dto.Categoria.Trim();
            p.Precio = dto.Precio;
            p.Cantidad = dto.Cantidad;
            return p;
        }
    }

    public bool Eliminar(int id)
    {
        lock (_lock) return _productos.RemoveAll(p => p.Id == id) > 0;
    }

    public ResumenInventario ObtenerResumen()
    {
        lock (_lock)
        {
            return new ResumenInventario(
                TotalProductos: _productos.Count,
                UnidadesEnInventario: _productos.Sum(p => p.Cantidad),
                ValorInventario: _productos.Sum(p => p.Precio * p.Cantidad),
                ProductosStockBajo: _productos.Count(p => p.Cantidad <= LimiteStockBajo));
        }
    }
}
