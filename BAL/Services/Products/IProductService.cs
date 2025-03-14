﻿using DPCV_API.Models.CommonModel;
using DPCV_API.Models.ProductModel;
using System.Security.Claims;

namespace DPCV_API.BAL.Services.Products
{
    public interface IProductService
    {
        Task<PaginatedResponse<ProductResponseDTO>> GetPaginatedProductsAsync(int pageNumber, int pageSize);
        Task<List<ProductResponseDTO>> GetAllProductsAsync();
        Task<ProductResponseDTO?> GetProductByIdAsync(int productId);
        Task<bool> CreateProductAsync(ProductDTO product, ClaimsPrincipal user);
        Task<bool> UpdateProductAsync(ProductDTO product, ClaimsPrincipal user);
        Task<bool> DeleteProductAsync(int productId, ClaimsPrincipal user);
        Task<(bool success, string message)> ArchiveProductAsync(int productId, ClaimsPrincipal user);
        Task<(bool success, string message)> UnarchiveProductAsync(int productId, ClaimsPrincipal user);
    }
}
