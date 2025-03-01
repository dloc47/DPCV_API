using DPCV_API.Models.EntityCount;

namespace DPCV_API.BAL.Services.EntityCount
{
    public interface IEntityService
    {
        Task<EntityCountDTO> GetEntityCountsAsync();
    }
}
