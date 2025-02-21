using DPCV_API.Models.CommitteeModel;

namespace DPCV_API.BAL.Services.Webiste.Committees
{
    public interface ICommitteeService
    {
        Task<List<VillageDTO>> GetAllVillageNamesAsync();
    }
}
