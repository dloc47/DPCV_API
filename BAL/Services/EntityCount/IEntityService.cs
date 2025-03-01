using DPCV_API.Models.Website.EntityCount;

namespace DPCV_API.BAL.Services.EntityCount
{
    public interface IEntityService
    {
        Task<EntityCountDTO> GetEntityCountsAsync();
    }
}
