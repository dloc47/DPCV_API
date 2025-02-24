using DPCV_API.Models.Website.CommitteeModel;

namespace DPCV_API.BAL.Services.Website.Committees
{
    public interface ICommitteeService
    {
        Task<List<VillageDTO>> GetAllVillageNamesAsync();
        Task<List<CommitteeDTO>> GetAllCommitteesAsync();
        Task<CommitteeDTO?> GetCommitteeByIdAsync(int committeeId);

    }
}
