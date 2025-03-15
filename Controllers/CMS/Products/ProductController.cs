using DPCV_API.BAL.Services.Products;
using DPCV_API.Models.ProductModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DPCV_API.Controllers.Website
{
    [Route("api/products")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly IProductService _productService;
        private readonly ILogger<ProductController> _logger;

        public ProductController(IProductService productService, ILogger<ProductController> logger)
        {
            _productService = productService;
            _logger = logger;
        }

        // ✅ Get Paginated Products
        [HttpGet("paginated-products")]
        public async Task<IActionResult> GetPaginatedProducts([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 5)
        {
            try
            {
                if (pageNumber < 1 || pageSize < 1)
                {
                    return BadRequest(new { message = "Invalid pagination parameters." });
                }

                var result = await _productService.GetPaginatedProductsAsync(pageNumber, pageSize);

                return result.Data.Any() ? Ok(result) : NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching paginated products.");
                return StatusCode(500, new { message = "An error occurred while fetching product data." });
            }
        }

        // ✅ Get All Products
        [HttpGet]
        public async Task<IActionResult> GetAllProducts()
        {
            var result = await _productService.GetAllProductsAsync();
            return Ok(result);
        }

        // ✅ Get Product by ID
        [HttpGet("{id}")]
        public async Task<IActionResult> GetProductById(int id)
        {
            var result = await _productService.GetProductByIdAsync(id);
            if (result == null)
            {
                return NotFound(new { message = "Product not found" });
            }
            return Ok(result);
        }

        // ✅ Create Product
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> CreateProduct([FromBody] ProductDTO productDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new { message = "Invalid request data.", errors = ModelState });
            }

            var result = await _productService.CreateProductAsync(productDto, User);

            if (!result)
            {
                return StatusCode(StatusCodes.Status403Forbidden,
                    new { message = "You are not authorized to create this Product." });
            }

            return Ok(new { message = "Product created successfully." });
        }

        // ✅ Update Product
        [Authorize]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateProduct(int id, [FromBody] ProductDTO productDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new { message = "Invalid request data.", errors = ModelState });
            }

            productDto.ProductId = id;
            var result = await _productService.UpdateProductAsync(productDto, User);

            if (!result)
            {
                return StatusCode(StatusCodes.Status403Forbidden,
                    new { message = "You are not authorized to update this Product or no changes were made." });
            }

            return Ok(new { message = "Product updated successfully." });
        }

        // ✅ Delete Product
        [Authorize]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            var result = await _productService.DeleteProductAsync(id, User);

            if (!result)
            {
                return StatusCode(StatusCodes.Status403Forbidden,
                    new { message = "You are not authorized to delete this Product or it does not exist." });
            }

            return Ok(new { message = "Product deleted successfully." });
        }

        // ✅ Archive Product (Set is_active = 0)
        [Authorize]
        [HttpPut("{id}/archive")]
        public async Task<IActionResult> ArchiveProduct(int id)
        {
            var (success, message) = await _productService.ArchiveProductAsync(id, User);

            if (!success)
                return StatusCode(StatusCodes.Status400BadRequest, new { message });

            return Ok(new { message });
        }

        // ✅ Unarchive Product (Set is_active = 1)
        [Authorize]
        [HttpPut("{id}/unarchive")]
        public async Task<IActionResult> UnarchiveProduct(int id)
        {
            var (success, message) = await _productService.UnarchiveProductAsync(id, User);

            if (!success)
                return StatusCode(StatusCodes.Status400BadRequest, new { message });

            return Ok(new { message });
        }
    }
}
