﻿using DPCV_API.Configuration.DbContext;
using DPCV_API.Helpers;
using DPCV_API.Models.CommonModel;
using DPCV_API.Models.ProductModel;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Data;
using System.Security.Claims;
using System.Text.Json;
using System.Threading.Tasks;

namespace DPCV_API.BAL.Services.Products
{
    public class ProductService : IProductService
    {
        private readonly DataManager _dataManager;
        private readonly ILogger<ProductService> _logger;

        public ProductService(DataManager dataManager, ILogger<ProductService> logger)
        {
            _dataManager = dataManager;
            _logger = logger;
        }

        // ✅ Get Paginated Products
        public async Task<PaginatedResponse<ProductResponseDTO>> GetPaginatedProductsAsync(int pageNumber, int pageSize)
        {
            string procedureName = "GetPaginatedProducts";

            var parameters = new Dictionary<string, object>
    {
        { "PageNumber", pageNumber },
        { "PageSize", pageSize }
    };

            DataTable result = await _dataManager.ExecuteQueryAsync(procedureName, CommandType.StoredProcedure, parameters);
            List<ProductResponseDTO> products = new();

            foreach (DataRow row in result.Rows)
            {
                products.Add(new ProductResponseDTO
                {
                    ProductId = Convert.ToInt32(row["product_id"]),
                    ProductName = row["product_name"].ToString()!,
                    Description = row["description"]?.ToString(),
                    MetricUnit = row["metric_unit"].ToString()!,
                    MetricValue = row["metric_value"] != DBNull.Value ? Convert.ToDecimal(row["metric_value"]) : null,
                    Price = Convert.ToDecimal(row["price"]),

                    CommitteeId = Convert.ToInt32(row["committee_id"]),
                    CommitteeName = row["committee_name"].ToString()!,
                    DistrictId = Convert.ToInt32(row["district_id"]),
                    DistrictName = row["district_name"].ToString()!,
                    Region = row["region"].ToString()!,

                    HomestayId = row["homestay_id"] != DBNull.Value ? Convert.ToInt32(row["homestay_id"]) : null,
                    HomestayName = row["homestay_id"] == DBNull.Value ? null : row["homestay_name"]?.ToString(),

                    Tags = JsonHelper.DeserializeJsonSafely<List<string>>(row["tags"], "tags"),
                    IsVerifiable = Convert.ToBoolean(row["isVerifiable"]),
                    VerificationStatusId = row["verification_status_id"] != DBNull.Value ? Convert.ToInt32(row["verification_status_id"]) : null,
                    StatusType = row["status_type"]?.ToString(),
                    IsActive = Convert.ToBoolean(row["is_active"])
                });
            }

            return new PaginatedResponse<ProductResponseDTO>
            {
                Data = products,
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalRecords = await GetTotalProductCountAsync()
            };
        }

        // ✅ Get total product count
        private async Task<int> GetTotalProductCountAsync()
        {
            string query = "SELECT COUNT(*) FROM products";

            object? result = await _dataManager.ExecuteScalarAsync<object>(query, CommandType.Text);

            return result != null ? Convert.ToInt32(result) : 0;
        }


        // ✅ Get All Products
        public async Task<List<ProductResponseDTO>> GetAllProductsAsync()
        {
            string spName = "GetAllProducts";
            List<ProductResponseDTO> products = new();

            try
            {
                DataTable result = await _dataManager.ExecuteQueryAsync(spName, CommandType.StoredProcedure);

                foreach (DataRow row in result.Rows)
                {
                    products.Add(new ProductResponseDTO
                    {
                        // Core Product Data
                        ProductId = Convert.ToInt32(row["product_id"]),
                        ProductName = row["product_name"].ToString()!,
                        Description = row["description"]?.ToString(),
                        MetricUnit = row["metric_unit"].ToString()!,
                        MetricValue = row["metric_value"] != DBNull.Value ? Convert.ToDecimal(row["metric_value"]) : null,
                        Price = Convert.ToDecimal(row["price"]),

                        // Committee & District Data
                        CommitteeId = Convert.ToInt32(row["committee_id"]),
                        CommitteeName = row["committee_name"].ToString()!,
                        DistrictId = Convert.ToInt32(row["district_id"]),
                        DistrictName = row["district_name"].ToString()!,
                        Region = row["region"].ToString()!,

                        // Homestay Data (if applicable)
                        HomestayId = row["homestay_id"] != DBNull.Value ? Convert.ToInt32(row["homestay_id"]) : null,
                        HomestayName = row["homestay_id"] == DBNull.Value ? null : row["homestay_name"]?.ToString(),

                        // Verification & Status
                        Tags = JsonHelper.DeserializeJsonSafely<List<string>>(row["tags"], "tags"),
                        IsVerifiable = Convert.ToBoolean(row["isVerifiable"]),
                        VerificationStatusId = row["verification_status_id"] != DBNull.Value ? Convert.ToInt32(row["verification_status_id"]) : null,
                        StatusType = row["status_type"]?.ToString(), // Fetching status type from master_verification_status
                        IsActive = Convert.ToBoolean(row["is_active"])
                    });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching products.");
                throw;
            }

            return products;
        }


        // ✅ Get Product by ID
        public async Task<ProductResponseDTO?> GetProductByIdAsync(int productId)
        {
            string spName = "GetProductById";

            try
            {
                _dataManager.ClearParameters();
                _dataManager.AddParameter("@p_product_id", productId);
                DataTable result = await _dataManager.ExecuteQueryAsync(spName, CommandType.StoredProcedure);

                if (result.Rows.Count == 0)
                {
                    _logger.LogWarning($"Product with ID {productId} not found.");
                    return null;
                }

                DataRow row = result.Rows[0];
                return new ProductResponseDTO
                {
                    // Core Product Data
                    ProductId = Convert.ToInt32(row["product_id"]),
                    ProductName = row["product_name"].ToString()!,
                    Description = row["description"]?.ToString(),
                    MetricUnit = row["metric_unit"].ToString()!,
                    MetricValue = row["metric_value"] != DBNull.Value ? Convert.ToDecimal(row["metric_value"]) : null,
                    Price = Convert.ToDecimal(row["price"]),

                    // Committee & District Data
                    CommitteeId = Convert.ToInt32(row["committee_id"]),
                    CommitteeName = row["committee_name"].ToString()!,
                    DistrictId = Convert.ToInt32(row["district_id"]),
                    DistrictName = row["district_name"].ToString()!,
                    Region = row["region"].ToString()!,

                    // Homestay Data (if applicable)
                    HomestayId = row["homestay_id"] != DBNull.Value ? Convert.ToInt32(row["homestay_id"]) : null,
                    HomestayName = row["homestay_id"] == DBNull.Value ? null : row["homestay_name"]?.ToString(),

                    // Verification & Status
                    Tags = JsonHelper.DeserializeJsonSafely<List<string>>(row["tags"], "tags"),
                    IsVerifiable = Convert.ToBoolean(row["isVerifiable"]),
                    VerificationStatusId = row["verification_status_id"] != DBNull.Value ? Convert.ToInt32(row["verification_status_id"]) : null,
                    StatusType = row["status_type"]?.ToString(), // Fetching status type from master_verification_status
                    IsActive = Convert.ToBoolean(row["is_active"])
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error fetching product with ID {productId}.");
                throw;
            }
        }


        // ✅ Create Product
        public async Task<bool> CreateProductAsync(ProductDTO product, ClaimsPrincipal user)
        {
            try
            {
                var roleClaim = user.FindFirst(ClaimTypes.Role);
                var committeeClaim = user.FindFirst("CommitteeId");

                int roleId = roleClaim != null ? int.Parse(roleClaim.Value) : 0;
                int? userCommitteeId = committeeClaim != null ? int.Parse(committeeClaim.Value) : null;

                if (roleId == 0 || roleId == 2 && userCommitteeId != product.CommitteeId)
                {
                    _logger.LogWarning("Unauthorized attempt to create a product.");
                    return false;
                }

                int verificationStatus = roleId == 1 ? 2 : 1;

                string procedureName = "CreateProduct";
                _dataManager.ClearParameters();
                _dataManager.AddParameter("@p_product_name", product.ProductName);
                _dataManager.AddParameter("@p_description", product.Description ?? (object)DBNull.Value);
                _dataManager.AddParameter("@p_metric_unit", product.MetricUnit);
                _dataManager.AddParameter("@p_metric_value", product.MetricValue ?? (object)DBNull.Value);
                _dataManager.AddParameter("@p_price", product.Price);
                _dataManager.AddParameter("@p_committee_id", product.CommitteeId);
                _dataManager.AddParameter("@p_homestay_id", product.HomestayId ?? (object)DBNull.Value);
                _dataManager.AddParameter("@p_tags", JsonSerializer.Serialize(product.Tags));
                _dataManager.AddParameter("@p_isVerifiable", product.IsVerifiable);
                _dataManager.AddParameter("@p_verification_status_id", verificationStatus);
                _dataManager.AddParameter("@p_is_active", product.IsActive);

                bool success = await _dataManager.ExecuteNonQueryAsync(procedureName, CommandType.StoredProcedure);
                if (success)
                {
                    _logger.LogInformation("Product created successfully: {ProductName}", product.ProductName);
                }
                return success;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating product: {ProductName}", product.ProductName);
                return false;
            }
        }

        // ✅ Update Product
        public async Task<bool> UpdateProductAsync(ProductDTO product, ClaimsPrincipal user)
        {
            try
            {
                var roleClaim = user.FindFirst(ClaimTypes.Role);
                var committeeClaim = user.FindFirst("CommitteeId");

                int roleId = roleClaim != null ? int.Parse(roleClaim.Value) : 0;
                int? userCommitteeId = committeeClaim != null ? int.Parse(committeeClaim.Value) : null;

                // Check if the product exists
                _dataManager.ClearParameters();
                _dataManager.AddParameter("@p_product_id", product.ProductId);
                DataTable dt = await _dataManager.ExecuteQueryAsync("GetProductById", CommandType.StoredProcedure);
                if (dt.Rows.Count == 0)
                    return false; // Product doesn't exist

                int existingCommitteeId = Convert.ToInt32(dt.Rows[0]["committee_id"]);

                // Committee users can only update their own products
                if (roleId == 2 && existingCommitteeId != userCommitteeId)
                {
                    _logger.LogWarning("Unauthorized attempt to update product: {ProductId}", product.ProductId);
                    return false;
                }

                int verificationStatus = roleId == 1 ? 2 : 1;

                _dataManager.ClearParameters();
                _dataManager.AddParameter("@p_product_id", product.ProductId);
                _dataManager.AddParameter("@p_product_name", product.ProductName);
                _dataManager.AddParameter("@p_description", product.Description ?? (object)DBNull.Value);
                _dataManager.AddParameter("@p_metric_unit", product.MetricUnit);
                _dataManager.AddParameter("@p_metric_value", product.MetricValue ?? (object)DBNull.Value);
                _dataManager.AddParameter("@p_price", product.Price);
                _dataManager.AddParameter("@p_committee_id", product.CommitteeId);
                _dataManager.AddParameter("@p_homestay_id", product.HomestayId ?? (object)DBNull.Value);
                _dataManager.AddParameter("@p_tags", JsonSerializer.Serialize(product.Tags));
                _dataManager.AddParameter("@p_isVerifiable", product.IsVerifiable);
                _dataManager.AddParameter("@p_verification_status_id", verificationStatus);
                _dataManager.AddParameter("@p_is_active", product.IsActive);

                bool success = await _dataManager.ExecuteNonQueryAsync("UpdateProduct", CommandType.StoredProcedure);
                if (success)
                {
                    _logger.LogInformation("Product updated successfully: {ProductId}", product.ProductId);
                }
                return success;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating product: {ProductId}", product.ProductId);
                return false;
            }
        }


        // ✅ Delete Product
        public async Task<bool> DeleteProductAsync(int productId, ClaimsPrincipal user)
        {
            try
            {
                var roleClaim = user.FindFirst(ClaimTypes.Role);
                var committeeClaim = user.FindFirst("CommitteeId");

                int roleId = roleClaim != null ? int.Parse(roleClaim.Value) : 0;
                int? userCommitteeId = committeeClaim != null ? int.Parse(committeeClaim.Value) : null;

                // Check if the product exists
                _dataManager.ClearParameters();
                _dataManager.AddParameter("@p_product_id", productId);
                DataTable dt = await _dataManager.ExecuteQueryAsync("GetProductById", CommandType.StoredProcedure);
                if (dt.Rows.Count == 0)
                    return false; // Product doesn't exist

                int existingCommitteeId = Convert.ToInt32(dt.Rows[0]["committee_id"]);

                // Committee users can only delete their own products
                if (roleId == 2 && existingCommitteeId != userCommitteeId)
                {
                    _logger.LogWarning("Unauthorized attempt to delete product: {ProductId}", productId);
                    return false;
                }

                _dataManager.ClearParameters();
                _dataManager.AddParameter("@p_product_id", productId);

                bool success = await _dataManager.ExecuteNonQueryAsync("DeleteProduct", CommandType.StoredProcedure);
                if (success)
                {
                    _logger.LogInformation("Product deleted successfully: {ProductId}", productId);
                }
                return success;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting product: {ProductId}", productId);
                return false;
            }
        }


        // ✅ Archive Product
        public async Task<(bool success, string message)> ArchiveProductAsync(int productId, ClaimsPrincipal user)
        {
            return await ToggleProductStatusAsync(productId, false, user);
        }

        // ✅ Unarchive Product
        public async Task<(bool success, string message)> UnarchiveProductAsync(int productId, ClaimsPrincipal user)
        {
            return await ToggleProductStatusAsync(productId, true, user);
        }

        // ✅ Toggle is_active (Helper Method)
        private async Task<(bool success, string message)> ToggleProductStatusAsync(int productId, bool isActive, ClaimsPrincipal user)
        {
            try
            {
                var roleClaim = user.FindFirst(ClaimTypes.Role);
                var committeeClaim = user.FindFirst("CommitteeId");

                int roleId = roleClaim != null ? int.Parse(roleClaim.Value) : 0;
                int? userCommitteeId = committeeClaim != null ? int.Parse(committeeClaim.Value) : null;

                _dataManager.ClearParameters();
                _dataManager.AddParameter("@p_product_id", productId);
                DataTable dt = await _dataManager.ExecuteQueryAsync("GetProductById", CommandType.StoredProcedure);
                if (dt.Rows.Count == 0)
                    return (false, "Product does not exist.");

                int existingCommitteeId = Convert.ToInt32(dt.Rows[0]["committee_id"]);
                bool currentStatus = Convert.ToBoolean(dt.Rows[0]["is_active"]);

                if (currentStatus == isActive)
                {
                    string alreadyMessage = isActive ? "Product is already active." : "Product is already archived.";
                    return (false, alreadyMessage);
                }

                if (roleId == 2 && existingCommitteeId != userCommitteeId)
                {
                    _logger.LogWarning("Unauthorized attempt to modify product status: {ProductId}", productId);
                    return (false, "You are not authorized to modify this product.");
                }

                _dataManager.ClearParameters();
                _dataManager.AddParameter("@p_product_id", productId);
                _dataManager.AddParameter("@p_is_active", isActive);

                bool success = await _dataManager.ExecuteNonQueryAsync("ToggleProductStatus", CommandType.StoredProcedure);
                return (success, isActive ? "Product unarchived successfully." : "Product archived successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating product status: {ProductId}", productId);
                return (false, "An error occurred while updating the product status.");
            }
        }
    }
}
