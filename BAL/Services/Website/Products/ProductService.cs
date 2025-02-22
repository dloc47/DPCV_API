using DPCV_API.Configuration.DbContext;
using DPCV_API.Models.Website.ProductModel;
using System.Data;
using System.Text.Json;

namespace DPCV_API.BAL.Services.Website.Products
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

        // ✅ Get All Products
        public async Task<List<ProductDTO>> GetAllProductsAsync()
        {
            var products = new List<ProductDTO>();

            try
            {
                string query = "SELECT * FROM products";
                DataTable dataTable = await _dataManager.ExecuteQueryAsync(query, CommandType.Text);

                foreach (DataRow row in dataTable.Rows)
                {
                    products.Add(MapProduct(row));
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching products.");
            }

            return products;
        }

        // ✅ Get Product By ID
        public async Task<ProductDTO?> GetProductByIdAsync(int productId)
        {
            try
            {
                string query = "SELECT * FROM products WHERE product_id = @p_product_id";
                _dataManager.ClearParameters();
                _dataManager.AddParameter("@p_product_id", productId);
                DataTable dataTable = await _dataManager.ExecuteQueryAsync(query, CommandType.Text);

                if (dataTable.Rows.Count == 0)
                    return null;

                return MapProduct(dataTable.Rows[0]);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching product with ID: {ProductId}", productId);
                return null;
            }
        }

        // ✅ Create Product
        public async Task<bool> CreateProductAsync(ProductDTO product)
        {
            try
            {
                string procedureName = "CreateProduct";
                _dataManager.ClearParameters();
                _dataManager.AddParameter("@p_product_name", product.ProductName);
                _dataManager.AddParameter("@p_description", product.Description ?? (object)DBNull.Value);
                _dataManager.AddParameter("@p_price", product.Price);
                _dataManager.AddParameter("@p_committee_id", product.CommitteeId);
                _dataManager.AddParameter("@p_homestay_id", product.HomestayId ?? (object)DBNull.Value);
                _dataManager.AddParameter("@p_tags", product.Tags?.RootElement.ToString() ?? (object)DBNull.Value);
                _dataManager.AddParameter("@p_isVerifiable", product.IsVerifiable);
                _dataManager.AddParameter("@p_verification_status_id", product.VerificationStatusId ?? (object)DBNull.Value);

                bool success = await _dataManager.ExecuteNonQueryAsync(procedureName, CommandType.StoredProcedure);

                if (success)
                    _logger.LogInformation("Product created successfully: {ProductName}", product.ProductName);

                return success;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating product: {ProductName}", product.ProductName);
                return false;
            }
        }

        // ✅ Update Product
        public async Task<bool> UpdateProductAsync(int productId, ProductDTO product)
        {
            try
            {
                if (await GetProductByIdAsync(productId) == null)
                {
                    _logger.LogWarning("Update failed. Product with ID {ProductId} does not exist.", productId);
                    return false;
                }

                string procedureName = "UpdateProduct";
                _dataManager.ClearParameters();
                _dataManager.AddParameter("@p_product_id", productId);
                _dataManager.AddParameter("@p_product_name", product.ProductName);
                _dataManager.AddParameter("@p_description", product.Description ?? (object)DBNull.Value);
                _dataManager.AddParameter("@p_price", product.Price);
                _dataManager.AddParameter("@p_committee_id", product.CommitteeId);
                _dataManager.AddParameter("@p_homestay_id", product.HomestayId ?? (object)DBNull.Value);
                _dataManager.AddParameter("@p_isVerifiable", product.IsVerifiable);
                _dataManager.AddParameter("@p_verification_status_id", product.VerificationStatusId ?? (object)DBNull.Value);

                bool success = await _dataManager.ExecuteNonQueryAsync(procedureName, CommandType.StoredProcedure);

                if (success)
                    _logger.LogInformation("Product updated successfully: {ProductId}", productId);

                return success;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating product with ID: {ProductId}", productId);
                return false;
            }
        }

        // ✅ Delete Product
        public async Task<bool> DeleteProductAsync(int productId)
        {
            try
            {
                if (await GetProductByIdAsync(productId) == null)
                {
                    _logger.LogWarning("Delete failed. Product with ID {ProductId} does not exist.", productId);
                    return false;
                }

                string query = "DELETE FROM products WHERE product_id = @p_product_id";
                _dataManager.ClearParameters();
                _dataManager.AddParameter("@p_product_id", productId);
                return await _dataManager.ExecuteNonQueryAsync(query, CommandType.Text);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting product with ID: {ProductId}", productId);
                return false;
            }
        }

        // ✅ Helper method to map DataRow to ProductDTO
        private ProductDTO MapProduct(DataRow row)
        {
            return new ProductDTO
            {
                ProductId = Convert.ToInt32(row["product_id"]),
                ProductName = row["product_name"].ToString() ?? string.Empty,
                Description = row["description"] as string,
                Price = Convert.ToDecimal(row["price"]),
                CommitteeId = Convert.ToInt32(row["committee_id"]),
                HomestayId = row["homestay_id"] == DBNull.Value ? null : Convert.ToInt32(row["homestay_id"]),
                Tags = row["tags"] == DBNull.Value ? null : JsonDocument.Parse(row["tags"].ToString()!),
                IsVerifiable = Convert.ToBoolean(row["isVerifiable"]),
                VerificationStatusId = row["verification_status_id"] == DBNull.Value ? null : Convert.ToInt32(row["verification_status_id"])
            };
        }
    }
}
