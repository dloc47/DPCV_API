using DPCV_API.BAL.Services.Website.Products;
using DPCV_API.Models.Website.ProductModel;
using Microsoft.AspNetCore.Mvc;

namespace DPCV_API.Controllers.Website.Product
{
    [Route("api/products")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly IProductService _productService;

        public ProductController(IProductService productService)
        {
            _productService = productService;
        }

        [HttpGet]
        public async Task<ActionResult<List<ProductDTO>>> GetAllProducts() => Ok(await _productService.GetAllProductsAsync());

        [HttpGet("{id}")]
        public async Task<ActionResult<ProductDTO>> GetProductById(int id)
        {
            var product = await _productService.GetProductByIdAsync(id);
            return product != null ? Ok(product) : NotFound("Product not found.");
        }

        [HttpPost]
        public async Task<ActionResult> CreateProduct([FromBody] ProductDTO product) => Ok(await _productService.CreateProductAsync(product));

        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateProduct(int id, [FromBody] ProductDTO product) => Ok(await _productService.UpdateProductAsync(id, product));

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteProduct(int id) => Ok(await _productService.DeleteProductAsync(id));
    }
}
