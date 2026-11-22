using System.ComponentModel.DataAnnotations;

namespace JHNegocio.Api.Models;

/// <summary>Producto del inventario de JH Negocio.</summary>
public class Producto
{
    public int Id { get; set; }
    public string Codigo { get; set; } = string.Empty;
    public string Nombre { get; set; } = string.Empty;
    public string Categoria { get; set; } = string.Empty;
    public decimal Precio { get; set; }
    public int Cantidad { get; set; }
    public DateTime FechaRegistro { get; set; } = DateTime.Now;
}

/// <summary>Datos que envía el cliente para crear o actualizar un producto.</summary>
public class ProductoDto
{
    [Required(ErrorMessage = "El código es obligatorio.")]
    [StringLength(20)]
    public string Codigo { get; set; } = string.Empty;

    [Required(ErrorMessage = "El nombre es obligatorio.")]
    [StringLength(80)]
    public string Nombre { get; set; } = string.Empty;

    [StringLength(40)]
    public string Categoria { get; set; } = "General";

    [Range(0.01, 1_000_000, ErrorMessage = "El precio debe ser mayor que 0.")]
    public decimal Precio { get; set; }

    [Range(0, 100_000, ErrorMessage = "La cantidad no puede ser negativa.")]
    public int Cantidad { get; set; }
}

/// <summary>Resumen del inventario para el panel de inicio.</summary>
public record ResumenInventario(
    int TotalProductos,
    int UnidadesEnInventario,
    decimal ValorInventario,
    int ProductosStockBajo);
