using DPCV_API.Configuration.DbContext;
using DPCV_API.Models.CommitteeModel;
using System.Data;

namespace DPCV_API.BAL.Services.Webiste.Committees
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
            }

            return committees;
        }
    }
}
