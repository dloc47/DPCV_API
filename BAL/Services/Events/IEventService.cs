using System.Security.Claims;
using DPCV_API.Models.CommonModel;
using DPCV_API.Models.EventModel;

namespace DPCV_API.BAL.Services.Events
{
    public interface IEventService
    {
        Task<PaginatedResponse<EventResponseDTO>> GetPaginatedEventsAsync(int pageNumber, int pageSize);
        Task<IEnumerable<EventResponseDTO>> GetAllEventsAsync(); // ✅ Use IEnumerable<T> for read-only collections
        Task<EventResponseDTO?> GetEventByIdAsync(int eventId);
        Task<bool> CreateEventAsync(EventDTO eventDto, ClaimsPrincipal user);
        Task<bool> UpdateEventAsync(EventDTO eventDto, ClaimsPrincipal user);
        Task<bool> DeleteEventAsync(int eventId, ClaimsPrincipal user);
        Task<(bool success, string message)> ArchiveEventAsync(int eventId, ClaimsPrincipal user);
        Task<(bool success, string message)> UnarchiveEventAsync(int eventId, ClaimsPrincipal user);
    }
}
