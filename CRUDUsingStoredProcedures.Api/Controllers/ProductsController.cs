using CRUDUsingStoredProcedures.Api.Contracts.Products;
using CRUDUsingStoredProcedures.Api.Interfaces;
using CRUDUsingStoredProcedures.Api.Models;
using Microsoft.AspNetCore.Mvc;

namespace CRUDUsingStoredProcedures.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductsController(IProductService productService) : ControllerBase
{
    private readonly IProductService _productService = productService;

    [HttpGet("")]
    public async Task<IActionResult> GetAll()
    {
        var products = await _productService.GetAllProductsAsync();
        return Ok(products);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var product = await _productService.GetProductByIdAsync(id);

        return product is null ? BadRequest() : Ok(product);
    }

    [HttpPost("")]
    public async Task<IActionResult> Add([FromBody] ProductRequest request)
    {
        var product = await _productService.AddProductAsync(new Product { Name = request.Name, Price = request.Price });

        return CreatedAtAction(nameof(GetById), new { id = product.Id }, product);
    }


    [HttpPut("")]
    public async Task<IActionResult> Update(int id,[FromBody] ProductRequest product)
    {
        await _productService.UpdateProductAsync(new Product { Id = id, Name = product.Name, Price = product.Price });

        return NoContent();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        await _productService.DeleteProductAsync(id);

        return Ok();
    }

}
