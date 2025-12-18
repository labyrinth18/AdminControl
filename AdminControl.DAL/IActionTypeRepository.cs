using AdminControl.DTO;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AdminControl.DAL
{
    public interface IActionTypeRepository
    {
        Task<IEnumerable<ActionTypeDto>> GetAllActionTypesAsync();
        Task<ActionTypeDto?> GetByIdAsync(int actionTypeId);
        Task<ActionTypeDto?> GetByNameAsync(string actionName);
    }
}
