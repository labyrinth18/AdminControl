using AdminControl.DAL;
using AdminControl.DALEF.Models;
using AdminControl.DTO;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AdminControl.DALEF.Concrete
{
    public class UserRepository : IUserRepository
    {
        private readonly AdminControlContext _context;

        public UserRepository(AdminControlContext context)
        {
            _context = context;
        }

        public async Task<UserDto> AddUserAsync(UserCreateDto newUserDto)
        {
            var passwordHash = newUserDto.Password.GetHashCode().ToString();

            var newUser = new User
            {
                Login = newUserDto.Login,
                PasswordHash = passwordHash,
                FirstName = newUserDto.FirstName,
                LastName = newUserDto.LastName,
                Email = newUserDto.Email,
                RoleID = newUserDto.RoleID,
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _context.Users.Add(newUser);
            await _context.SaveChangesAsync();

            var createdUser = await _context.Users
                .Include(u => u.Role)
                .FirstAsync(u => u.UserID == newUser.UserID);

            return new UserDto
            {
                UserID = createdUser.UserID,
                Login = createdUser.Login,
                FirstName = createdUser.FirstName,
                LastName = createdUser.LastName,
                Email = createdUser.Email,
                RoleName = createdUser.Role.RoleName
            };
        }

        public async Task<IEnumerable<UserDto>> GetAllUsersAsync()
        {
            var usersFromDb = await _context.Users
                .Include(user => user.Role)
                .ToListAsync();

            var usersDto = usersFromDb.Select(user => new UserDto
            {
                UserID = user.UserID,
                Login = user.Login,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                RoleName = user.Role.RoleName
            }).ToList();

            return usersDto;
        }

        public async Task UpdateUserAsync(UserUpdateDto userToUpdateDto)
        {
            var userFromDb = await _context.Users.FindAsync(userToUpdateDto.UserID);

            if (userFromDb != null)
            {
                userFromDb.FirstName = userToUpdateDto.FirstName;
                userFromDb.LastName = userToUpdateDto.LastName;
                userFromDb.Email = userToUpdateDto.Email;
                userFromDb.RoleID = userToUpdateDto.RoleID;
                userFromDb.UpdatedAt = DateTime.UtcNow;

                await _context.SaveChangesAsync();
            }
        }

        public async Task DeleteUserAsync(int userId)
        {
            var userToDelete = await _context.Users.FindAsync(userId);

            if (userToDelete != null)
            {
                _context.Users.Remove(userToDelete);
                await _context.SaveChangesAsync();
            }
        }
    }
}