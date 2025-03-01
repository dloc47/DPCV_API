using System.Security.Claims;
using DPCV_API.Models.CMS.Users;

public interface IUserService
{
    Task<List<UserDTO>> GetAllUsersAsync();
    Task<UserDTO> GetUserByIdAsync(int userId);
    Task<bool> CreateUserAsync(UserDTO userDto, ClaimsPrincipal user);
    Task<bool> UpdateUserAsync(UserDTO userDto, ClaimsPrincipal user);
    Task<bool> DeleteUserAsync(int userId, ClaimsPrincipal user);
    Task<(bool success, string message)> ArchiveUserAsync(int committeeId, ClaimsPrincipal user);
    Task<(bool success, string message)> UnarchiveUserAsync(int committeeId, ClaimsPrincipal user);
}
