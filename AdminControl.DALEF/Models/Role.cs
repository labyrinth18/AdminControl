using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AdminControl.DALEF.Models
{
    [Table("Roles")] // Явно вказуємо назву таблицi
    public class Role
    {
        [Key]
        public int RoleID { get; set; }

        [Required] 
        [MaxLength(50)] 
        public string RoleName { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; } 
    }
}