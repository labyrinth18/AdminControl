using AdminControl.DTO;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AdminControl.DAL
{
    public interface IUserRepository
    {
        Task<IEnumerable<UserDto>> GetAllUsersAsync();
        Task<UserDto> AddUserAsync(UserCreateDto newUser);
        Task UpdateUserAsync(UserUpdateDto userToUpdate);
        Task DeleteUserAsync(int userId);
        Task<UserDto?> AuthenticateUserAsync(string login, string passwordHash);
        
        // Методи для валідації унікальності
        Task<bool> IsLoginExistsAsync(string login);
        Task<bool> IsEmailExistsAsync(string email, int? excludeUserId = null);
    }
}
