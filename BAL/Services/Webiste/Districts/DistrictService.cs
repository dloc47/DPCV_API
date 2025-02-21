using DPCV_API.Configuration.DbContext;
using DPCV_API.Models.DistrictModel;
using System.Data;

namespace DPCV_API.Services.DistrictService
{
    public class DistrictService : IDistrictService
    {
        private readonly DataManager _dataManager;
        private readonly ILogger<DistrictService> _logger; // Add logger

        // Inject DataManager via constructor
        public DistrictService(DataManager dataManager, ILogger<DistrictService> logger)
        {
            _dataManager = dataManager;
            _logger = logger;
        }

        // ✅ Get All Districts
        public async Task<List<DistrictDTO>> GetAllDistrictsAsync()
        {
            string query = "SELECT * FROM districts";
            DataTable result = await _dataManager.ExecuteQueryAsync(query, CommandType.Text);
            List<DistrictDTO> districts = new();

            foreach (DataRow row in result.Rows)
            {
                districts.Add(new DistrictDTO
                {
                    DistrictId = Convert.ToInt32(row["district_id"]),
                    DistrictName = row["district_name"].ToString()!,
                    Region = row["region"].ToString()!
                });
            }
            return districts;
        }

        // ✅ Get District by ID
        public async Task<DistrictDTO?> GetDistrictByIdAsync(int districtId)
        {
            string query = "SELECT * FROM districts WHERE district_id = @DistrictId";
            _dataManager.ClearParameters();
            _dataManager.AddParameter("@DistrictId", districtId);

            DataTable result = await _dataManager.ExecuteQueryAsync(query, CommandType.Text);
            if (result.Rows.Count == 0) return null;

            DataRow row = result.Rows[0];
            return new DistrictDTO
            {
                DistrictId = Convert.ToInt32(row["district_id"]),
                DistrictName = row["district_name"].ToString()!,
                Region = row["region"].ToString()!
            };
        }

        public async Task<bool> CreateDistrictAsync(DistrictDTO district)
        {
            try
            {
                string procedureName = "CreateDistrict";
                _dataManager.ClearParameters();
                _dataManager.AddParameter("@p_district_name", district.DistrictName);
                _dataManager.AddParameter("@p_region", district.Region);

                bool success = await _dataManager.ExecuteNonQueryAsync(procedureName, CommandType.StoredProcedure);
                if (success)
                {
                    _logger.LogInformation("District created successfully: {DistrictName}", district.DistrictName);
                }
                return success;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating district: {DistrictName}", district.DistrictName);
                return false;
            }
        }

        public async Task<bool> UpdateDistrictAsync(int districtId, DistrictDTO district)
        {
            try
            {
                string procedureName = "UpdateDistrict";
                _dataManager.ClearParameters();

                _dataManager.AddParameter("@p_district_id", districtId);
                // Only send non-null and non-empty parameters
                _dataManager.AddParameter("@p_district_name", string.IsNullOrWhiteSpace(district.DistrictName) ? DBNull.Value : district.DistrictName);
                _dataManager.AddParameter("@p_region", string.IsNullOrWhiteSpace(district.Region) ? DBNull.Value : district.Region);

                bool success = await _dataManager.ExecuteNonQueryAsync(procedureName, CommandType.StoredProcedure);

                if (success)
                    _logger.LogInformation("District updated successfully: {DistrictId}", districtId);
                else
                    _logger.LogWarning("No district record was updated for: {DistrictId}", districtId);

                return success;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating district: {DistrictId}", districtId);
                return false;
            }
        }


    }
}
