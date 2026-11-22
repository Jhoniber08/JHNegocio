namespace JHNegocio.Movil.Models;

/// <summary>Producto tal como lo devuelve la API (JSON → C#).</summary>
public class Producto
{
    public int Id { get; set; }
    public string Codigo { get; set; } = string.Empty;
    public string Nombre { get; set; } = string.Empty;
    public string Categoria { get; set; } = string.Empty;
    public decimal Precio { get; set; }
    public int Cantidad { get; set; }
    public DateTime FechaRegistro { get; set; }

    // Propiedades calculadas para la interfaz
    public decimal ValorTotal => Precio * Cantidad;
    public bool StockBajo => Cantidad <= 5;
    public string PrecioTexto => $"RD$ {Precio:N2}";
    public string CantidadTexto => StockBajo ? $"{Cantidad} und · Stock bajo" : $"{Cantidad} und";
}

/// <summary>Datos que se envían a la API para crear un producto.</summary>
public class ProductoDto
{
    public string Codigo { get; set; } = string.Empty;
    public string Nombre { get; set; } = string.Empty;
    public string Categoria { get; set; } = "General";
    public decimal Precio { get; set; }
    public int Cantidad { get; set; }
}

/// <summary>Resumen del inventario (GET /api/productos/resumen).</summary>
public class ResumenInventario
{
    public int TotalProductos { get; set; }
    public int UnidadesEnInventario { get; set; }
    public decimal ValorInventario { get; set; }
    public int ProductosStockBajo { get; set; }
}
