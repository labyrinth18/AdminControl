using AdminControl.DALEF.Models;
using Microsoft.EntityFrameworkCore;

namespace AdminControl.DALEF.Concrete
{
    public class AdminControlContext : DbContext
    {
        public AdminControlContext(DbContextOptions<AdminControlContext> options) : base(options)
        {
        }

        public DbSet<Role> Roles { get; set; } = null!;
        public DbSet<User> Users { get; set; } = null!;
        public DbSet<UserBankCard> UserBankCards { get; set; } = null!;
        public DbSet<ActionType> ActionTypes { get; set; } = null!;
        public DbSet<AdminActionLog> AdminActionLogs { get; set; } = null!; 
    }
}