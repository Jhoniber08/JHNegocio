using JHNegocio.Api.Data;
using JHNegocio.Api.Models;
using Microsoft.AspNetCore.Mvc;

namespace JHNegocio.Api.Controllers;

/// <summary>Endpoints REST de productos: /api/productos</summary>
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class ProductosController : ControllerBase
{
    private readonly IProductoRepository _repo;

    public ProductosController(IProductoRepository repo) => _repo = repo;

    /// <summary>GET /api/productos?buscar=texto — Lista todos los productos.</summary>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<Producto>), StatusCodes.Status200OK)]
    public ActionResult<IEnumerable<Producto>> ObtenerTodos([FromQuery] string? buscar) =>
        Ok(_repo.ObtenerTodos(buscar));

    /// <summary>GET /api/productos/5 — Obtiene un producto por Id.</summary>
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(Producto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public ActionResult<Producto> ObtenerPorId(int id)
    {
        var producto = _repo.ObtenerPorId(id);
        return producto is null
            ? NotFound(new { mensaje = $"No existe un producto con Id {id}." })
            : Ok(producto);
    }

    /// <summary>GET /api/productos/resumen — Totales del inventario.</summary>
    [HttpGet("resumen")]
    [ProducesResponseType(typeof(ResumenInventario), StatusCodes.Status200OK)]
    public ActionResult<ResumenInventario> Resumen() => Ok(_repo.ObtenerResumen());

    /// <summary>POST /api/productos — Crea un producto.</summary>
    [HttpPost]
    [ProducesResponseType(typeof(Producto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public ActionResult<Producto> Crear([FromBody] ProductoDto dto)
    {
        var creado = _repo.Crear(dto);
        return CreatedAtAction(nameof(ObtenerPorId), new { id = creado.Id }, creado);
    }

    /// <summary>PUT /api/productos/5 — Actualiza un producto.</summary>
    [HttpPut("{id:int}")]
    [ProducesResponseType(typeof(Producto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public ActionResult<Producto> Actualizar(int id, [FromBody] ProductoDto dto)
    {
        var actualizado = _repo.Actualizar(id, dto);
        return actualizado is null
            ? NotFound(new { mensaje = $"No existe un producto con Id {id}." })
            : Ok(actualizado);
    }

    /// <summary>DELETE /api/productos/5 — Elimina un producto.</summary>
    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult Eliminar(int id) =>
        _repo.Eliminar(id)
            ? NoContent()
            : NotFound(new { mensaje = $"No existe un producto con Id {id}." });
}
