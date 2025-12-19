using AdminControl.DTO;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AdminControl.BLL.Interfaces
{
    public interface IRoleService
    {
        Task<IEnumerable<RoleDto>> GetAllAsync();
        Task<RoleDto> GetByIdAsync(int id);
        Task CreateAsync(RoleCreateDto roleDto);
        Task UpdateAsync(RoleUpdateDto roleDto);
        Task DeleteAsync(int id);
    }
}