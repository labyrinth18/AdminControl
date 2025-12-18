using AdminControl.DAL;
using AdminControl.DALEF.Models;
using AdminControl.DTO;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
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

        public async Task<IEnumerable<UserDto>> GetAllUsersAsync()
        {
            var usersFromDb = await _context.Users
                .Include(user => user.Role)
                .ToListAsync();

            return usersFromDb.Select(user => new UserDto
            {
                UserID = user.UserID,
                Login = user.Login,
                FirstName = user.FirstName ?? string.Empty,
                LastName = user.LastName ?? string.Empty,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                Address = user.Address,
                Gender = user.Gender,
                RoleID = user.RoleID,
                RoleName = user.Role?.RoleName ?? "Немає ролі",
                IsActive = user.IsActive
            }).ToList();
        }

        public async Task<UserDto> AddUserAsync(UserCreateDto newUserDto)
        {
            // Хешування паролю SHA256
            string passwordHash;
            using (var sha256 = SHA256.Create())
            {
                var bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(newUserDto.Password));
                passwordHash = BitConverter.ToString(bytes).Replace("-", "").ToLower();
            }

            var newUser = new User
            {
                Login = newUserDto.Login,
                PasswordHash = passwordHash,
                FirstName = newUserDto.FirstName,
                LastName = newUserDto.LastName,
                Email = newUserDto.Email,
                PhoneNumber = newUserDto.PhoneNumber,
                Address = newUserDto.Address,
                Gender = newUserDto.Gender,
                RoleID = newUserDto.RoleID,
                IsActive = newUserDto.IsActive,
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
                FirstName = createdUser.FirstName ?? string.Empty,
                LastName = createdUser.LastName ?? string.Empty,
                Email = createdUser.Email,
                PhoneNumber = createdUser.PhoneNumber,
                Address = createdUser.Address,
                Gender = createdUser.Gender,
                RoleID = createdUser.RoleID,
                RoleName = createdUser.Role?.RoleName ?? string.Empty,
                IsActive = createdUser.IsActive
            };
        }

        public async Task UpdateUserAsync(UserUpdateDto userToUpdateDto)
        {
            var userFromDb = await _context.Users.FindAsync(userToUpdateDto.UserID);

            if (userFromDb != null)
            {
                userFromDb.FirstName = userToUpdateDto.FirstName;
                userFromDb.LastName = userToUpdateDto.LastName;
                userFromDb.Email = userToUpdateDto.Email;
                userFromDb.PhoneNumber = userToUpdateDto.PhoneNumber;
                userFromDb.Address = userToUpdateDto.Address;
                userFromDb.Gender = userToUpdateDto.Gender;
                userFromDb.RoleID = userToUpdateDto.RoleID;
                userFromDb.IsActive = userToUpdateDto.IsActive;
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

        public async Task<UserDto?> AuthenticateUserAsync(string login, string passwordHash)
        {
            var user = await _context.Users
                .Include(u => u.Role)
                .FirstOrDefaultAsync(u => u.Login == login && u.PasswordHash == passwordHash);

            if (user == null) return null;

            return new UserDto
            {
                UserID = user.UserID,
                Login = user.Login,
                FirstName = user.FirstName ?? string.Empty,
                LastName = user.LastName ?? string.Empty,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                Address = user.Address,
                Gender = user.Gender,
                RoleID = user.RoleID,
                RoleName = user.Role?.RoleName ?? string.Empty,
                IsActive = user.IsActive
            };
        }

        public async Task<bool> IsLoginExistsAsync(string login)
        {
            return await _context.Users.AnyAsync(u => u.Login == login);
        }

        public async Task<bool> IsEmailExistsAsync(string email, int? excludeUserId = null)
        {
            if (excludeUserId.HasValue)
            {
                return await _context.Users.AnyAsync(u => u.Email == email && u.UserID != excludeUserId.Value);
            }
            return await _context.Users.AnyAsync(u => u.Email == email);
        }
    }
}
