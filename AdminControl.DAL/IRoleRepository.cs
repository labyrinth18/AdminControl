using AdminControl.DTO;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AdminControl.DAL
{
    public interface IRoleRepository
    {

        Task<IEnumerable<RoleDto>> GetAllRolesAsync();

        Task<RoleDto> AddRoleAsync(RoleCreateDto newRole);

        Task UpdateRoleAsync(RoleUpdateDto roleToUpdate);

        Task DeleteRoleAsync(int roleId);
    }
}