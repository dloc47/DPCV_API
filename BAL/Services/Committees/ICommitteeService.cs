using System.Security.Claims;
using DPCV_API.Models.CommitteeModel;
using DPCV_API.Models.CommonModel;

namespace DPCV_API.BAL.Services.Committees
{
    public interface ICommitteeService
    {
        Task<List<VillageDTO>> GetAllVillageNamesAsync();
        Task<PaginatedResponse<CommitteeResponseDTO>> GetPaginatedCommitteesAsync(int pageNumber, int pageSize);
        Task<List<CommitteeResponseDTO>> GetAllCommitteesAsync();
        Task<CommitteeResponseDTO?> GetCommitteeByIdAsync(int committeeId);
        Task<bool> CreateCommitteeAsync(CommitteeDTO committeeDto, ClaimsPrincipal user);
        Task<bool> UpdateCommitteeAsync(CommitteeDTO committeeDto, ClaimsPrincipal user);
        Task<bool> DeleteCommitteeAsync(int committeeId, ClaimsPrincipal user);
        Task<(bool success, string message)> ArchiveCommitteeAsync(int committeeId, ClaimsPrincipal user);
        Task<(bool success, string message)> UnarchiveCommitteeAsync(int committeeId, ClaimsPrincipal user);

    }
}
