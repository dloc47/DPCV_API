using DPCV_API.Configuration.DbContext;
using DPCV_API.Models.CommitteeModel;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Data;
using System.Security.Claims;
using System.Text.Json;
using System.Threading.Tasks;

namespace DPCV_API.BAL.Services.Committees
{
    public class CommitteeService : ICommitteeService
    {
        private readonly DataManager _dataManager;
        private readonly ILogger<CommitteeService> _logger;

        public CommitteeService(DataManager dataManager, ILogger<CommitteeService> logger)
        {
            _dataManager = dataManager;
            _logger = logger;
        }

        private static T DeserializeJsonSafely<T>(object dbValue, string fieldName)
        {
            if (dbValue == DBNull.Value || dbValue == null || string.IsNullOrWhiteSpace(dbValue.ToString()))
            {
                return Activator.CreateInstance<T>(); // Return an empty object of the required type
            }

            try
            {
                return JsonSerializer.Deserialize<T>(dbValue.ToString()!, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                })!;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error deserializing {fieldName}: {ex.Message}");
                return Activator.CreateInstance<T>(); // Return an empty object on failure
            }
        }

        public async Task<List<VillageDTO>> GetAllVillageNamesAsync()
        {
            string spName = "GetAllVillageNames";
            List<VillageDTO> committees = new();

            try
            {
                DataTable result = await _dataManager.ExecuteQueryAsync(spName, CommandType.StoredProcedure);

                foreach (DataRow row in result.Rows)
                {
                    committees.Add(new VillageDTO
                    {
                        CommitteeId = Convert.ToInt32(row["committee_id"]),
                        CommitteeName = row["committee_name"].ToString()!,
                        DistrictId = Convert.ToInt32(row["district_id"]),
                        VerificationStatusId = row["verification_status_id"] != DBNull.Value ? Convert.ToInt32(row["verification_status_id"]) : null,
                        is_active = Convert.ToInt32(row["is_active"])
                    });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching village names.");
                throw;
            }

            return committees;
        }

        public async Task<List<CommitteeResponseDTO>> GetAllCommitteesAsync()
        {
            string spName = "GetAllCommittees";
            List<CommitteeResponseDTO> committees = new();

            try
            {
                DataTable result = await _dataManager.ExecuteQueryAsync(spName, CommandType.StoredProcedure);

                foreach (DataRow row in result.Rows)
                {
                    committees.Add(new CommitteeResponseDTO
                    {
                        CommitteeId = Convert.ToInt32(row["committee_id"]),
                        CommitteeName = row["committee_name"].ToString()!,
                        Description = row["description"]?.ToString(),
                        DistrictId = Convert.ToInt32(row["district_id"]),
                        ContactNumber = row["contact_number"]?.ToString(),
                        Email = row["email"]?.ToString(),
                        Address = row["address"]?.ToString(),
                        Tags = DeserializeJsonSafely<List<string>>(row["tags"], "tags"),
                        TouristAttractions = DeserializeJsonSafely<List<TouristAttraction>>(row["tourist_attractions"], "tourist_attractions"),
                        Latitude = row["latitude"] != DBNull.Value ? Convert.ToDecimal(row["latitude"]) : null,
                        Longitude = row["longitude"] != DBNull.Value ? Convert.ToDecimal(row["longitude"]) : null,
                        Leadership = DeserializeJsonSafely<List<LeadershipMember>>(row["leadership"], "leadership"),
                        IsVerifiable = Convert.ToInt32(row["isVerifiable"]),
                        VerificationStatusId = row["verification_status_id"] != DBNull.Value ? Convert.ToInt32(row["verification_status_id"]) : null,
                        IsActive = Convert.ToInt32(row["is_active"]),
                        VerificationStatus = row["verification_status"]?.ToString(),
                        DistrictName = row["district_name"]?.ToString()
                    });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching committees.");
                throw;
            }

            return committees;
        }


        public async Task<CommitteeResponseDTO?> GetCommitteeByIdAsync(int committeeId)
        {
            string spName = "GetCommitteeById";

            try
            {
                _dataManager.ClearParameters();
                _dataManager.AddParameter("@p_committee_id", committeeId);
                DataTable result = await _dataManager.ExecuteQueryAsync(spName, CommandType.StoredProcedure);

                if (result.Rows.Count == 0)
                {
                    _logger.LogWarning($"Committee with ID {committeeId} not found.");
                    return null;
                }

                DataRow row = result.Rows[0];
                return new CommitteeResponseDTO
                {
                    CommitteeId = Convert.ToInt32(row["committee_id"]),
                    CommitteeName = row["committee_name"].ToString()!,
                    Description = row["description"]?.ToString(),
                    DistrictId = Convert.ToInt32(row["district_id"]),
                    ContactNumber = row["contact_number"]?.ToString(),
                    Email = row["email"]?.ToString(),
                    Address = row["address"]?.ToString(),
                    Tags = DeserializeJsonSafely<List<string>>(row["tags"], "tags"),
                    TouristAttractions = DeserializeJsonSafely<List<TouristAttraction>>(row["tourist_attractions"], "tourist_attractions"),
                    Latitude = row["latitude"] != DBNull.Value ? Convert.ToDecimal(row["latitude"]) : null,
                    Longitude = row["longitude"] != DBNull.Value ? Convert.ToDecimal(row["longitude"]) : null,
                    Leadership = DeserializeJsonSafely<List<LeadershipMember>>(row["leadership"], "leadership"),
                    IsVerifiable = Convert.ToInt32(row["isVerifiable"]),
                    VerificationStatusId = row["verification_status_id"] != DBNull.Value ? Convert.ToInt32(row["verification_status_id"]) : null,
                    IsActive = Convert.ToInt32(row["is_active"]),
                    VerificationStatus = row["verification_status"]?.ToString(),
                    DistrictName = row["district_name"]?.ToString()
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error fetching committee with ID {committeeId}.");
                throw;
            }
        }


        public async Task<bool> CreateCommitteeAsync(CommitteeDTO committeeDto, ClaimsPrincipal user)
        {
            try
            {
                var roleClaim = user.FindFirst(ClaimTypes.Role);
                var committeeClaim = user.FindFirst("CommitteeId");

                int roleId = roleClaim != null ? int.Parse(roleClaim.Value) : 0;
                int? committeeId = committeeClaim != null ? int.Parse(committeeClaim.Value) : null;

                if (roleId == 0 || (roleId == 2 && committeeId != committeeDto.CommitteeId))
                {
                    _logger.LogWarning("Unauthorized attempt to create a committee.");
                    return false;
                }

                int verificationStatus = roleId == 1 ? 2 : 1;

                string procedureName = "CreateCommittee";
                _dataManager.ClearParameters();
                _dataManager.AddParameter("@p_committee_name", committeeDto.CommitteeName);
                _dataManager.AddParameter("@p_description", committeeDto.Description ?? (object)DBNull.Value);
                _dataManager.AddParameter("@p_district_id", committeeDto.DistrictId);
                _dataManager.AddParameter("@p_contact_number", committeeDto.ContactNumber ?? (object)DBNull.Value);
                _dataManager.AddParameter("@p_email", committeeDto.Email ?? (object)DBNull.Value);
                _dataManager.AddParameter("@p_address", committeeDto.Address ?? (object)DBNull.Value);
                _dataManager.AddParameter("@p_tags", JsonSerializer.Serialize(committeeDto.Tags));
                _dataManager.AddParameter("@p_tourist_attractions", JsonSerializer.Serialize(committeeDto.TouristAttractions));
                _dataManager.AddParameter("@p_latitude", committeeDto.Latitude ?? (object)DBNull.Value);
                _dataManager.AddParameter("@p_longitude", committeeDto.Longitude ?? (object)DBNull.Value);
                _dataManager.AddParameter("@p_leadership", JsonSerializer.Serialize(committeeDto.Leadership));
                _dataManager.AddParameter("@p_isVerifiable", committeeDto.IsVerifiable);
                _dataManager.AddParameter("@p_verification_status_id", verificationStatus);
                _dataManager.AddParameter("@p_is_active", committeeDto.IsActive);

                bool success = await _dataManager.ExecuteNonQueryAsync(procedureName, CommandType.StoredProcedure);
                if (success)
                {
                    _logger.LogInformation("Committee created successfully: {CommitteeName}", committeeDto.CommitteeName);
                }
                return success;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating committee: {CommitteeName}", committeeDto.CommitteeName);
                return false;
            }
        }


        public async Task<bool> UpdateCommitteeAsync(CommitteeDTO committeeDto, ClaimsPrincipal user)
        {
            try
            {
                // Extract role and committee ID from claims
                var roleClaim = user.FindFirst(ClaimTypes.Role);
                var committeeClaim = user.FindFirst("CommitteeId");

                int roleId = roleClaim != null ? int.Parse(roleClaim.Value) : 0;
                int? committeeId = committeeClaim != null ? int.Parse(committeeClaim.Value) : null;

                _logger.LogInformation("User Role: {RoleId}, User CommitteeId: {CommitteeId}", roleId, committeeId);

                // Fetch existing committee data
                _dataManager.ClearParameters();
                _dataManager.AddParameter("p_committee_id", committeeDto.CommitteeId);
                DataTable dt = await _dataManager.ExecuteQueryAsync("GetCommitteeById", CommandType.StoredProcedure);

                if (dt.Rows.Count == 0)
                {
                    _logger.LogWarning("Committee not found: {CommitteeId}", committeeDto.CommitteeId);
                    return false;
                }

                int existingCommitteeId = Convert.ToInt32(dt.Rows[0]["committee_id"]);

                // Authorization check: Committee users can only edit their own committee
                if (roleId == 2 && existingCommitteeId != committeeId)
                {
                    _logger.LogWarning("Unauthorized update attempt. User CommitteeId: {UserCommitteeId}, Requested CommitteeId: {RequestedCommitteeId}",
                        committeeId, committeeDto.CommitteeId);
                    return false;
                }

                // Admin sets verification status to 2 (Verified), Committee user sets it to 1 (Pending)
                int verificationStatus = roleId == 1 ? 2 : 1;

                // Prepare stored procedure parameters
                string procedureName = "UpdateCommittee";
                _dataManager.ClearParameters();
                _dataManager.AddParameter("p_committee_id", committeeDto.CommitteeId);
                _dataManager.AddParameter("p_committee_name", committeeDto.CommitteeName);
                _dataManager.AddParameter("p_description", committeeDto.Description ?? (object)DBNull.Value);
                _dataManager.AddParameter("p_district_id", committeeDto.DistrictId);
                _dataManager.AddParameter("p_contact_number", committeeDto.ContactNumber ?? (object)DBNull.Value);
                _dataManager.AddParameter("p_email", committeeDto.Email ?? (object)DBNull.Value);
                _dataManager.AddParameter("p_address", committeeDto.Address ?? (object)DBNull.Value);
                _dataManager.AddParameter("p_tags", JsonSerializer.Serialize(committeeDto.Tags));
                _dataManager.AddParameter("p_tourist_attractions", JsonSerializer.Serialize(committeeDto.TouristAttractions));
                _dataManager.AddParameter("p_latitude", committeeDto.Latitude ?? (object)DBNull.Value);
                _dataManager.AddParameter("p_longitude", committeeDto.Longitude ?? (object)DBNull.Value);
                _dataManager.AddParameter("p_leadership", JsonSerializer.Serialize(committeeDto.Leadership));
                _dataManager.AddParameter("p_isVerifiable", committeeDto.IsVerifiable);
                _dataManager.AddParameter("p_verification_status_id", verificationStatus);
                _dataManager.AddParameter("p_is_active", committeeDto.IsActive);

                _logger.LogInformation("Executing stored procedure {ProcedureName} for CommitteeId: {CommitteeId}", procedureName, committeeDto.CommitteeId);

                bool success = await _dataManager.ExecuteNonQueryAsync(procedureName, CommandType.StoredProcedure);

                if (success)
                {
                    _logger.LogInformation("Committee updated successfully: {CommitteeId}", committeeDto.CommitteeId);
                    return true;
                }
                else
                {
                    _logger.LogWarning("No committee record was updated for: {CommitteeId}", committeeDto.CommitteeId);
                    return false;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating committee: {CommitteeId}", committeeDto.CommitteeId);
                return false;
            }
        }


        public async Task<bool> DeleteCommitteeAsync(int committeeId, ClaimsPrincipal user)
        {
            try
            {
                var roleClaim = user.FindFirst(ClaimTypes.Role);
                var committeeClaim = user.FindFirst("CommitteeId");

                int roleId = roleClaim != null ? int.Parse(roleClaim.Value) : 0;
                int? committeeIdUser = committeeClaim != null ? int.Parse(committeeClaim.Value) : null;

                _dataManager.ClearParameters();
                _dataManager.AddParameter("p_committee_id", committeeId);
                DataTable dt = await _dataManager.ExecuteQueryAsync("GetCommitteeById", CommandType.StoredProcedure);
                if (dt.Rows.Count == 0) return false;

                int existingCommitteeId = Convert.ToInt32(dt.Rows[0]["committee_id"]);
                if (roleId == 2 && existingCommitteeId != committeeIdUser)
                {
                    _logger.LogWarning("Unauthorized attempt to delete committee: {CommitteeId}", committeeId);
                    return false;
                }

                _dataManager.ClearParameters();
                _dataManager.AddParameter("p_committee_id", committeeId);
                bool success = await _dataManager.ExecuteNonQueryAsync("DeleteCommittee", CommandType.StoredProcedure);
                if (success)
                    _logger.LogInformation("Committee deleted successfully: {CommitteeId}", committeeId);
                else
                    _logger.LogWarning("No committee record was deleted for: {CommitteeId}", committeeId);

                return success;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting committee: {CommitteeId}", committeeId);
                return false;
            }
        }

        // ✅ Archive Committee (Set is_active = 0)
        public async Task<(bool success, string message)> ArchiveCommitteeAsync(int committeeId, ClaimsPrincipal user)
        {
            return await ToggleCommitteeStatusAsync(committeeId, false, user);
        }

        // ✅ Unarchive Committee (Set is_active = 1)
        public async Task<(bool success, string message)> UnarchiveCommitteeAsync(int committeeId, ClaimsPrincipal user)
        {
            return await ToggleCommitteeStatusAsync(committeeId, true, user);
        }

        // ✅ Helper Method to Toggle is_active
        private async Task<(bool success, string message)> ToggleCommitteeStatusAsync(int committeeId, bool isActive, ClaimsPrincipal user)
        {
            try
            {
                var roleClaim = user.FindFirst(ClaimTypes.Role);
                var committeeClaim = user.FindFirst("CommitteeId");

                int roleId = roleClaim != null ? int.Parse(roleClaim.Value) : 0;
                int? userCommitteeId = committeeClaim != null ? int.Parse(committeeClaim.Value) : null;

                // Check if the committee exists and get current is_active status
                _dataManager.ClearParameters();
                _dataManager.AddParameter("p_committee_id", committeeId);
                DataTable dt = await _dataManager.ExecuteQueryAsync("GetCommitteeById", CommandType.StoredProcedure);
                if (dt.Rows.Count == 0)
                    return (false, "Committee does not exist.");

                int existingCommitteeId = Convert.ToInt32(dt.Rows[0]["committee_id"]);
                bool currentStatus = Convert.ToBoolean(dt.Rows[0]["is_active"]);

                // If the status is already set, return an appropriate message
                if (currentStatus == isActive)
                {
                    string alreadyMessage = isActive ? "Committee is already active." : "Committee is already archived.";
                    return (false, alreadyMessage);
                }

                if (roleId == 2 && existingCommitteeId != userCommitteeId)
                {
                    _logger.LogWarning("Unauthorized attempt to modify committee status: {CommitteeId}", committeeId);
                    return (false, "You are not authorized to modify this committee.");
                }

                // Call stored procedure to update is_active
                _dataManager.ClearParameters();
                _dataManager.AddParameter("@p_committee_id", committeeId);
                _dataManager.AddParameter("@p_is_active", isActive);

                DataTable result = await _dataManager.ExecuteQueryAsync("ToggleCommitteeStatus", CommandType.StoredProcedure);
                bool success = result.Rows.Count > 0 && Convert.ToInt32(result.Rows[0]["success"]) == 1;

                if (success)
                {
                    _logger.LogInformation("Committee status updated successfully: {CommitteeId}, Active: {IsActive}", committeeId, isActive);
                    return (true, isActive ? "Committee unarchived successfully." : "Committee archived successfully.");
                }
                else
                {
                    _logger.LogWarning("No committee record was updated for: {CommitteeId}", committeeId);
                    return (false, "Failed to update committee status.");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating committee status: {CommitteeId}", committeeId);
                return (false, "An error occurred while updating the committee status.");
            }
        }

    }
}
