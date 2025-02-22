using DPCV_API.Models.Website.EntityCount;

namespace DPCV_API.BAL.Services.Website.EntityCount
{
    public interface IEntityService
    {
        Task<EntityCountDTO> GetEntityCountsAsync();
    }
}
