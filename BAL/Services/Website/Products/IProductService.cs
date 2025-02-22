using DPCV_API.Models.Website.ProductModel;

namespace DPCV_API.BAL.Services.Website.Products
{
    public interface IProductService
    {
        Task<List<ProductDTO>> GetAllProductsAsync();
        Task<ProductDTO?> GetProductByIdAsync(int productId);
        Task<bool> CreateProductAsync(ProductDTO product);
        Task<bool> UpdateProductAsync(int productId, ProductDTO product);
        Task<bool> DeleteProductAsync(int productId);
    }
}
