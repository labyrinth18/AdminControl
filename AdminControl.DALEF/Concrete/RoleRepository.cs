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
    public class RoleRepository : IRoleRepository
    {
        private readonly AdminControlContext _context;

        public RoleRepository(AdminControlContext context)
        {
            _context = context;
        }

        public async Task<RoleDto> AddRoleAsync(RoleCreateDto newRoleDto)
        {
            var newRole = new Role
            {
                RoleName = newRoleDto.RoleName,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _context.Roles.Add(newRole);
            await _context.SaveChangesAsync();

            return new RoleDto
            {
                RoleID = newRole.RoleID,
                RoleName = newRole.RoleName
            };
        }

        public async Task<IEnumerable<RoleDto>> GetAllRolesAsync()
        {
            var rolesFromDb = await _context.Roles.ToListAsync();

            var rolesDto = rolesFromDb.Select(role => new RoleDto
            {
                RoleID = role.RoleID,
                RoleName = role.RoleName
            });

            return rolesDto;
        }

        public async Task UpdateRoleAsync(RoleUpdateDto roleToUpdateDto)
        {
            var roleFromDb = await _context.Roles.FindAsync(roleToUpdateDto.RoleID);

            if (roleFromDb != null)
            {
                roleFromDb.RoleName = roleToUpdateDto.RoleName;
                roleFromDb.UpdatedAt = DateTime.UtcNow;

                await _context.SaveChangesAsync();
            }
        }

        public async Task DeleteRoleAsync(int roleId)
        {
            var roleToDelete = await _context.Roles.FindAsync(roleId);

            if (roleToDelete != null)
            {
                _context.Roles.Remove(roleToDelete);
                await _context.SaveChangesAsync();
            }
        }
    }
}