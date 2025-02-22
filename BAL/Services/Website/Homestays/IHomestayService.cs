using DPCV_API.Models.Website.HomestayModel;

namespace DPCV_API.BAL.Services.Website.Homestays
{
    public interface IHomestayService
    {
        Task<List<HomestayDTO>> GetAllHomestaysAsync();
        Task<HomestayDTO?> GetHomestayByIdAsync(int homestayId);
        Task<bool> CreateHomestayAsync(HomestayDTO homestay);
        Task<bool> UpdateHomestayAsync(int homestayId, HomestayDTO homestay);
    }

}
