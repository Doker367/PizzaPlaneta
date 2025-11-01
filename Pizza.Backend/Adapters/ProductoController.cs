using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Pizza.Backend.Application.DTOs;
using Pizza.Backend.Ports;
using Pizza.Backend.Domain;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Pizza.Backend.Adapters;

[ApiController]
[Route("api/productos")]
public class ProductoController : ControllerBase
{
    private readonly IProductService _productService;

    public ProductoController(IProductService productService)
    {
        _productService = productService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Producto>>> GetAll()
    {
        var productos = await _productService.GetAllAsync();
        return Ok(productos);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Producto>> GetById(int id)
    {
        var producto = await _productService.GetByIdAsync(id);
        if (producto == null)
        {
            return NotFound();
        }
        return Ok(producto);
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<Producto>> Create(CreateProductDto productDto)
    {
        var producto = await _productService.CreateAsync(productDto);
        return CreatedAtAction(nameof(GetById), new { id = producto.Id }, producto);
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Update(int id, UpdateProductDto productDto)
    {
        try
        {
            var updatedProduct = await _productService.UpdateAsync(id, productDto);
            return Ok(updatedProduct);
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(int id)
    {
        var existingProduct = await _productService.GetByIdAsync(id);
        if (existingProduct == null)
        {
            return NotFound();
        }
        await _productService.DeleteAsync(id);
        return NoContent();
    }
}
