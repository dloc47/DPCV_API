using DPCV_API.Configuration.DbContext;
using DPCV_API.Models.Website.DistrictModel;
using System.Data;
using System.Security.Claims;

namespace DPCV_API.BAL.Services.Districts
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
            string spName = "GetAllDistricts";
            List<DistrictDTO> districts = new();

            try
            {
                DataTable result = await _dataManager.ExecuteQueryAsync(spName, CommandType.StoredProcedure);

                foreach (DataRow row in result.Rows)
                {
                    districts.Add(new DistrictDTO
                    {
                        DistrictId = Convert.ToInt32(row["district_id"]),
                        DistrictName = row["district_name"].ToString()!,
                        Region = row["region"].ToString()!
                    });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching districts.");
                throw;
            }

            return districts;
        }

        // ✅ Get District by ID
        public async Task<DistrictDTO?> GetDistrictByIdAsync(int districtId)
        {
            string spName = "GetDistrictById";

            try
            {
                _dataManager.ClearParameters();
                _dataManager.AddParameter("@districtId", districtId);
                DataTable result = await _dataManager.ExecuteQueryAsync(spName, CommandType.StoredProcedure);

                if (result.Rows.Count == 0)
                {
                    _logger.LogWarning($"District with ID {districtId} not found.");
                    return null;
                }

                DataRow row = result.Rows[0];
                return new DistrictDTO
                {
                    DistrictId = Convert.ToInt32(row["district_id"]),
                    DistrictName = row["district_name"].ToString()!,
                    Region = row["region"].ToString()!
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error fetching district with ID {districtId}.");
                throw;
            }
        }

        public async Task<bool> CreateDistrictAsync(DistrictDTO district, ClaimsPrincipal user)
        {
            try
            {
                var roleClaim = user.FindFirst(ClaimTypes.Role);
                int roleId = roleClaim != null ? int.Parse(roleClaim.Value) : 0;

                if (roleId != 1)
                {
                    _logger.LogWarning("Unauthorized attempt to create a district.");
                    return false;
                }

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

        public async Task<bool> UpdateDistrictAsync(DistrictDTO district, ClaimsPrincipal user)
        {
            try
            {
                var roleClaim = user.FindFirst(ClaimTypes.Role);
                int roleId = roleClaim != null ? int.Parse(roleClaim.Value) : 0;

                if (roleId != 1)
                {
                    _logger.LogWarning("Unauthorized attempt to update a district.");
                    return false;
                }

                string procedureName = "UpdateDistrict";
                _dataManager.ClearParameters();
                _dataManager.AddParameter("@p_district_id", district.DistrictId);

                if (!string.IsNullOrWhiteSpace(district.DistrictName))
                    _dataManager.AddParameter("@p_district_name", district.DistrictName);
                if (!string.IsNullOrWhiteSpace(district.Region))
                    _dataManager.AddParameter("@p_region", district.Region);

                bool success = await _dataManager.ExecuteNonQueryAsync(procedureName, CommandType.StoredProcedure);
                if (success)
                {
                    _logger.LogInformation("District updated successfully: {DistrictId}", district.DistrictId);
                }
                else
                {
                    _logger.LogWarning("No district record was updated for: {DistrictId}", district.DistrictId);
                }
                return success;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating district: {DistrictId}", district.DistrictId);
                return false;
            }
        }



    }
}
