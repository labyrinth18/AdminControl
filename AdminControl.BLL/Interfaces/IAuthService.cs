using AdminControl.DTO;
using System.Threading.Tasks;

namespace AdminControl.BLL.Interfaces
{
    public interface IAuthService
    {
        Task<UserDto> AuthenticateAsync(string login, string password);
    }
}