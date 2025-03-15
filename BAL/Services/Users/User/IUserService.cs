using System.Security.Claims;
using DPCV_API.Models.CommonModel;
using DPCV_API.Models.Users;

public interface IUserService
{
    Task<PaginatedResponse<UserResponseDTO>> GetPaginatedUsersAsync(int pageNumber, int pageSize);
    Task<List<UserResponseDTO>?> GetAllUsersAsync();
    Task<UserResponseDTO?> GetUserByIdAsync(int userId);
    Task<bool> CreateUserAsync(UserDTO userDto, ClaimsPrincipal user);
    Task<bool> UpdateUserAsync(UserDTO userDto, ClaimsPrincipal user);
    Task<bool> DeleteUserAsync(int userId, ClaimsPrincipal user);
    Task<(bool success, string message)> ArchiveUserAsync(int committeeId, ClaimsPrincipal user);
    Task<(bool success, string message)> UnarchiveUserAsync(int committeeId, ClaimsPrincipal user);
}
