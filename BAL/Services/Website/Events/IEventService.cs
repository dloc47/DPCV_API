using DPCV_API.Models.Website.EventModel;

namespace DPCV_API.BAL.Services.Website.Events
{
    public interface IEventService
    {
        Task<IEnumerable<EventDTO>> GetAllEventsAsync(); // ✅ Use IEnumerable<T> for read-only collections
        Task<EventDTO?> GetEventByIdAsync(int eventId);
        Task<bool> CreateEventAsync(EventDTO eventDto);
        Task<bool> UpdateEventAsync(int eventId, EventDTO eventDto);
        Task<bool> DeleteEventAsync(int eventId);
    }
}
