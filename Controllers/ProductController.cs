using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PaymentAPI.Infrastructure;
using PaymentAPI.Models;
using PaymentAPI.DTO;
using PaymentAPI.Primitives;
using Microsoft.EntityFrameworkCore;

namespace PaymentAPI.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly ApplicationDbContext _db;

        public ProductController(ApplicationDbContext db)
        {
            _db = db;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var products = await _db.Products.ToListAsync();
            return Ok(products);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var product = await _db.Products.FindAsync(new ProductId(id));
            if (product is null) return NotFound();
            return Ok(product);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateProductRequest request)
        {
            var product = new Product(request.Name, request.Price, request.Description, request.StockQuantity);
            _db.Products.Add(product);
            await _db.SaveChangesAsync();
            return CreatedAtAction(nameof(GetById), new { id = product.Id }, product);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var product = await _db.Products.FindAsync(new ProductId(id));
            if (product is null) return NotFound();
            _db.Products.Remove(product);
            await _db.SaveChangesAsync();
            return NoContent();
        }

        [HttpPost("{id}/stock/add")]
        public async Task<IActionResult> AddStock(Guid id, [FromBody] StockRequest request)
        {
            var product = await _db.Products.FindAsync(new ProductId(id));
            if (product is null) return NotFound();
            product.AddToStock(request.Quantity);
            await _db.SaveChangesAsync();
            return Ok(product);
        }

        [HttpPost("{id}/stock/remove")]
        public async Task<IActionResult> RemoveStock(Guid id, [FromBody] StockRequest request)
        {
            var product = await _db.Products.FindAsync(new ProductId(id));
            if (product is null) return NotFound();
            product.RemoveFromStock(request.Quantity);
            await _db.SaveChangesAsync();
            return Ok(product);
        }
    }
}
