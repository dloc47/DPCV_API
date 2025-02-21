using DPCV_API.Models;

public interface IUserService
{
    Task<List<UserDTO>> GetAllUsersAsync();
    Task<UserDTO> GetUserByIdAsync(int userId);
    Task<bool> CreateUserAsync(UserDTO userDto);
    Task<bool> UpdateUserAsync(int userId, UserDTO userDto);
    Task<bool> DeleteUserAsync(int userId);
    Task<bool> SetUserActiveStatusAsync(int userId, bool isActive);
}
