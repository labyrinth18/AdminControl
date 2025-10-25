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
    }
}