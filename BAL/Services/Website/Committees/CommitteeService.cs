using DPCV_API.Configuration.DbContext;
using DPCV_API.Models.Website.CommitteeModel;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text.Json;
using System.Threading.Tasks;

namespace DPCV_API.BAL.Services.Website.Committees
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

        public async Task<List<VillageDTO>> GetAllVillageNamesAsync()
        {
            string query = "SELECT committee_id, committee_name, district_id, verification_status_id FROM committees";
            List<VillageDTO> committees = new();

            try
            {
                DataTable result = await _dataManager.ExecuteQueryAsync(query, CommandType.Text);

                foreach (DataRow row in result.Rows)
                {
                    committees.Add(new VillageDTO
                    {
                        CommitteeId = Convert.ToInt32(row["committee_id"]),
                        CommitteeName = row["committee_name"].ToString()!,
                        DistrictId = Convert.ToInt32(row["district_id"]),
                        VerificationStatusId = row["verification_status_id"] != DBNull.Value ? Convert.ToInt32(row["verification_status_id"]) : null
                    });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching village names.");
                throw new ApplicationException("Failed to fetch village names. Please try again later.");
            }

            return committees;
        }

        public async Task<List<CommitteeDTO>> GetAllCommitteesAsync()
        {
            string query = "SELECT * FROM committees";

            List<CommitteeDTO> homestayCommittees = new();

            try
            {
                DataTable result = await _dataManager.ExecuteQueryAsync(query, CommandType.Text);

                foreach (DataRow row in result.Rows)
                {
                    homestayCommittees.Add(new CommitteeDTO
                    {
                        CommitteeId = Convert.ToInt32(row["committee_id"]),
                        CommitteeName = row["committee_name"].ToString()!,
                        DistrictId = Convert.ToInt32(row["district_id"]),
                        ContactNumber = row["contact_number"]?.ToString(),
                        Email = row["email"]?.ToString(),
                        Address = row["address"]?.ToString(),
                        Tags = row["tags"] != DBNull.Value ? JsonDocument.Parse(row["tags"].ToString()!) : null,
                        IsVerifiable = Convert.ToBoolean(row["isVerifiable"]),
                        VerificationStatusId = row["verification_status_id"] != DBNull.Value ? Convert.ToInt32(row["verification_status_id"]) : null
                    });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching committees.");
                throw new ApplicationException("Failed to fetch committees. Please try again later.");
            }

            return homestayCommittees;
        }

        public async Task<CommitteeDTO?> GetCommitteeByIdAsync(int committeeId)
        {
            string query = @"
                SELECT 
                    committee_id, committee_name, district_id, 
                    contact_number, email, address, tags, 
                    isVerifiable, verification_status_id 
                FROM committees WHERE committee_id = @CommitteeId";

            try
            {
                _dataManager.ClearParameters();
                _dataManager.AddParameter("@CommitteeId", committeeId);
                DataTable result = await _dataManager.ExecuteQueryAsync(query, CommandType.Text);

                if (result.Rows.Count == 0)
                {
                    _logger.LogWarning($"Committee with ID {committeeId} not found.");
                    return null;
                }

                DataRow row = result.Rows[0];
                return new CommitteeDTO
                {
                    CommitteeId = Convert.ToInt32(row["committee_id"]),
                    CommitteeName = row["committee_name"].ToString()!,
                    DistrictId = Convert.ToInt32(row["district_id"]),
                    ContactNumber = row["contact_number"]?.ToString(),
                    Email = row["email"]?.ToString(),
                    Address = row["address"]?.ToString(),
                    Tags = row["tags"] != DBNull.Value ? JsonDocument.Parse(row["tags"].ToString()!) : null,
                    IsVerifiable = Convert.ToBoolean(row["isVerifiable"]),
                    VerificationStatusId = row["verification_status_id"] != DBNull.Value ? Convert.ToInt32(row["verification_status_id"]) : null
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error fetching committee with ID {committeeId}.");
                throw new ApplicationException("Failed to fetch committee details. Please try again later.");
            }
        }
    }
}
