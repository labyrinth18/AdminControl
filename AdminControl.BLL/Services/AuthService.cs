using AdminControl.BLL.Interfaces;
using AdminControl.DAL;
using AdminControl.DTO;
using System;
using System.Security.Cryptography; 
using System.Text;                  
using System.Threading.Tasks;

namespace AdminControl.BLL.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepository;

        public AuthService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<UserDto> AuthenticateAsync(string login, string password)
        {

            string passwordHash;
            using (var sha256 = SHA256.Create())
            {
                var bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                passwordHash = BitConverter.ToString(bytes).Replace("-", "").ToLower();
            }
            var user = await _userRepository.AuthenticateUserAsync(login, passwordHash);

            if (user == null)
            {
                throw new Exception("Невірний логін або пароль.");
            }

            return user;
        }
    }
}