using DPCV_API.BAL.Services.Website.Homestays;
using DPCV_API.Configuration.DbContext;
using DPCV_API.Models.Website.HomestayModel;
using Microsoft.Extensions.Logging;
using System.Data;
using System.Text.Json;

namespace DPCV_API.BAL.Services.Website.Homestays
{
    public class HomestayService : IHomestayService
    {
        private readonly DataManager _dataManager;
        private readonly ILogger<HomestayService> _logger;

        public HomestayService(DataManager dataManager, ILogger<HomestayService> logger)
        {
            _dataManager = dataManager;
            _logger = logger;
        }

        // ✅ Get All Homestays
        public async Task<List<HomestayDTO>> GetAllHomestaysAsync()
        {
            string query = "SELECT * FROM homestays";
            DataTable result = await _dataManager.ExecuteQueryAsync(query, CommandType.Text);
            List<HomestayDTO> homestays = new();

            foreach (DataRow row in result.Rows)
            {
                homestays.Add(new HomestayDTO
                {
                    HomestayId = Convert.ToInt32(row["homestay_id"]),
                    HomestayName = row["homestay_name"].ToString()!,
                    CommitteeId = Convert.ToInt32(row["committee_id"]),
                    Address = row["address"].ToString()!,
                    OwnerName = row["owner_name"].ToString()!,
                    OwnerMobile = row["owner_mobile"].ToString()!,
                    TotalRooms = Convert.ToInt32(row["total_rooms"]),
                    RoomTariff = Convert.ToDecimal(row["room_tariff"]),
                    Tags = row["tags"] != DBNull.Value ? JsonDocument.Parse(row["tags"].ToString()!) : null,
                    IsVerifiable = Convert.ToBoolean(row["isVerifiable"]),
                    VerificationStatusId = row["verification_status_id"] != DBNull.Value ? Convert.ToInt32(row["verification_status_id"]) : null
                });
            }
            return homestays;
        }

        // ✅ Get Homestay by ID
        public async Task<HomestayDTO?> GetHomestayByIdAsync(int homestayId)
        {
            string query = "SELECT * FROM homestays WHERE homestay_id = @HomestayId";
            _dataManager.ClearParameters();
            _dataManager.AddParameter("@HomestayId", homestayId);

            DataTable result = await _dataManager.ExecuteQueryAsync(query, CommandType.Text);
            if (result.Rows.Count == 0) return null;

            DataRow row = result.Rows[0];
            return new HomestayDTO
            {
                HomestayId = Convert.ToInt32(row["homestay_id"]),
                HomestayName = row["homestay_name"].ToString()!,
                CommitteeId = Convert.ToInt32(row["committee_id"]),
                Address = row["address"].ToString()!,
                OwnerName = row["owner_name"].ToString()!,
                OwnerMobile = row["owner_mobile"].ToString()!,
                TotalRooms = Convert.ToInt32(row["total_rooms"]),
                RoomTariff = Convert.ToDecimal(row["room_tariff"]),
                Tags = row["tags"] != DBNull.Value ? JsonDocument.Parse(row["tags"].ToString()!) : null,
                IsVerifiable = Convert.ToBoolean(row["isVerifiable"]),
                VerificationStatusId = row["verification_status_id"] != DBNull.Value ? Convert.ToInt32(row["verification_status_id"]) : null
            };
        }

        // ✅ Create Homestay
        public async Task<bool> CreateHomestayAsync(HomestayDTO homestay)
        {
            try
            {
                string procedureName = "CreateHomestay";
                _dataManager.ClearParameters();
                _dataManager.AddParameter("@p_homestay_name", homestay.HomestayName);
                _dataManager.AddParameter("@p_committee_id", homestay.CommitteeId);
                _dataManager.AddParameter("@p_address", homestay.Address);
                _dataManager.AddParameter("@p_owner_name", homestay.OwnerName);
                _dataManager.AddParameter("@p_owner_mobile", homestay.OwnerMobile);
                _dataManager.AddParameter("@p_total_rooms", homestay.TotalRooms);
                _dataManager.AddParameter("@p_room_tariff", homestay.RoomTariff);
                _dataManager.AddParameter("@p_tags", homestay.Tags?.RootElement.ToString() ?? (object)DBNull.Value);
                _dataManager.AddParameter("@p_isVerifiable", homestay.IsVerifiable);
                _dataManager.AddParameter("@p_verification_status_id", homestay.VerificationStatusId.HasValue ? homestay.VerificationStatusId : (object)DBNull.Value);

                bool success = await _dataManager.ExecuteNonQueryAsync(procedureName, CommandType.StoredProcedure);
                if (success)
                {
                    _logger.LogInformation("Homestay created successfully: {HomestayName}", homestay.HomestayName);
                }
                return success;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating homestay: {HomestayName}", homestay.HomestayName);
                return false;
            }
        }

        // ✅ Update Homestay
        public async Task<bool> UpdateHomestayAsync(int homestayId, HomestayDTO homestay)
        {
            try
            {
                string procedureName = "UpdateHomestay";
                _dataManager.ClearParameters();
                _dataManager.AddParameter("@p_homestay_id", homestayId);
                _dataManager.AddParameter("@p_homestay_name", homestay.HomestayName);
                _dataManager.AddParameter("@p_committee_id", homestay.CommitteeId);
                _dataManager.AddParameter("@p_address", homestay.Address);
                _dataManager.AddParameter("@p_owner_name", homestay.OwnerName);
                _dataManager.AddParameter("@p_owner_mobile", homestay.OwnerMobile);
                _dataManager.AddParameter("@p_total_rooms", homestay.TotalRooms);
                _dataManager.AddParameter("@p_room_tariff", homestay.RoomTariff);
                _dataManager.AddParameter("@p_tags", homestay.Tags?.RootElement.ToString() ?? (object)DBNull.Value);
                _dataManager.AddParameter("@p_isVerifiable", homestay.IsVerifiable);
                _dataManager.AddParameter("@p_verification_status_id", homestay.VerificationStatusId.HasValue ? homestay.VerificationStatusId : (object)DBNull.Value);

                bool success = await _dataManager.ExecuteNonQueryAsync(procedureName, CommandType.StoredProcedure);
                if (success)
                {
                    _logger.LogInformation("Homestay updated successfully: {HomestayId}", homestayId);
                }
                return success;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating homestay: {HomestayId}", homestayId);
                return false;
            }
        }
    }
}
