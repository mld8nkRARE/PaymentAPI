using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PaymentAPI.Infrastructure;
using PaymentAPI.Primitives;
using Microsoft.EntityFrameworkCore;
using PaymentAPI.DTO.product;
using PaymentAPI.Domain;

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

        /// <summary>
        /// Получает список всех продуктов.
        /// </summary>
        /// <param name="includeDeleted">Включать ли удаленные продукты.</param>
        /// <returns>Список продуктов.</returns>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<Product>))]
        public async Task<IActionResult> GetAll([FromQuery] bool includeDeleted = false)
        {
            var query = _db.Products.AsQueryable();
            if (!includeDeleted)
                query = query.Where(p => !p.IsDeleted);
            var products = await query.ToListAsync();
            return Ok(products);
        }

        /// <summary>
        /// Получает продукт по его идентификатору.
        /// </summary>
        /// <param name="id">Идентификатор продукта.</param>
        /// <returns>Продукт.</returns>
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Product))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetById(Guid id)
        {
            var product = await _db.Products.FindAsync(new ProductId(id));
            if (product is null) return NotFound();
            return Ok(product);
        }

        /// <summary>
        /// Создает новый продукт.
        /// </summary>
        /// <param name="request">Данные для создания продукта.</param>
        /// <returns>Созданный продукт.</returns>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(Product))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Create([FromBody] ProductCreateRequest request)
        {
            var product = new Product(request.Name, request.Price, request.Description, request.StockQuantity);
            _db.Products.Add(product);
            await _db.SaveChangesAsync();
            return CreatedAtAction(nameof(GetById), new { id = product.Id }, product);
        }

        /// <summary>
        /// Помечает продукт как удаленный.
        /// </summary>
        /// <param name="id">Идентификатор продукта.</param>
        /// <returns>Статус выполнения операции.</returns>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(Guid id)
        {
            var product = await _db.Products.FindAsync(new ProductId(id));
            if (product is null) return NotFound();
            product.MarkAsDeleted();
            await _db.SaveChangesAsync();
            return NoContent();
        }

        /// <summary>
        /// Добавляет количество товара на склад.
        /// </summary>
        /// <param name="id">Идентификатор продукта.</param>
        /// <param name="request">Количество для добавления.</param>
        /// <returns>Обновленный продукт.</returns>
        [HttpPost("{id}/stock/add")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Product))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> AddStock(Guid id, [FromBody] ProductStockRequest request)
        {
            var product = await _db.Products.FindAsync(new ProductId(id));
            if (product is null) return NotFound();
            product.AddToStock(request.Quantity);
            await _db.SaveChangesAsync();
            return Ok(product);
        }

        /// <summary>
        /// Удаляет количество товара со склада.
        /// </summary>
        /// <param name="id">Идентификатор продукта.</param>
        /// <param name="request">Количество для удаления.</param>
        /// <returns>Обновленный продукт.</returns>
        [HttpPost("{id}/stock/remove")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Product))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> RemoveStock(Guid id, [FromBody] ProductStockRequest request)
        {
            var product = await _db.Products.FindAsync(new ProductId(id));
            if (product is null) return NotFound();
            product.RemoveFromStock(request.Quantity);
            await _db.SaveChangesAsync();
            return Ok(product);
        }
    }
}
