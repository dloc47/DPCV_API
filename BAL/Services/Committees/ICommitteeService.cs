using System.Security.Claims;
using DPCV_API.Models.CommitteeModel;

namespace DPCV_API.BAL.Services.Committees
{
    public interface ICommitteeService
    {
        Task<List<VillageDTO>> GetAllVillageNamesAsync();
        Task<List<CommitteeDTO>> GetAllCommitteesAsync();
        Task<CommitteeDTO?> GetCommitteeByIdAsync(int committeeId);
        Task<bool> CreateCommitteeAsync(CommitteeDTO committeeDto, ClaimsPrincipal user);
        Task<bool> UpdateCommitteeAsync(CommitteeDTO committeeDto, ClaimsPrincipal user);
        Task<bool> DeleteCommitteeAsync(int committeeId, ClaimsPrincipal user);
        Task<(bool success, string message)> ArchiveCommitteeAsync(int committeeId, ClaimsPrincipal user);
        Task<(bool success, string message)> UnarchiveCommitteeAsync(int committeeId, ClaimsPrincipal user);

    }
}
