using AdminControl.DAL;
using AdminControl.DTO;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AdminControl.DALEF.Concrete
{
    public class ActionTypeRepository : IActionTypeRepository
    {
        private readonly AdminControlContext _context;

        public ActionTypeRepository(AdminControlContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<ActionTypeDto>> GetAllActionTypesAsync()
        {
            return await _context.ActionTypes
                .Select(at => new ActionTypeDto
                {
                    ActionTypeID = at.ActionTypeID,
                    ActionName = at.ActionName,
                    ActionDescription = at.ActionDescription ?? string.Empty
                })
                .OrderBy(at => at.ActionDescription)
                .ToListAsync();
        }

        public async Task<ActionTypeDto?> GetByIdAsync(int actionTypeId)
        {
            var actionType = await _context.ActionTypes.FindAsync(actionTypeId);
            
            if (actionType == null) return null;

            return new ActionTypeDto
            {
                ActionTypeID = actionType.ActionTypeID,
                ActionName = actionType.ActionName,
                ActionDescription = actionType.ActionDescription ?? string.Empty
            };
        }

        public async Task<ActionTypeDto?> GetByNameAsync(string actionName)
        {
            var actionType = await _context.ActionTypes
                .FirstOrDefaultAsync(at => at.ActionName == actionName);
            
            if (actionType == null) return null;

            return new ActionTypeDto
            {
                ActionTypeID = actionType.ActionTypeID,
                ActionName = actionType.ActionName,
                ActionDescription = actionType.ActionDescription ?? string.Empty
            };
        }
    }
}
