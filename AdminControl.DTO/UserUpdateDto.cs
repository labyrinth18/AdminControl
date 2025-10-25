using System.ComponentModel.DataAnnotations;

namespace AdminControl.DTO
{
    public class UserUpdateDto
    {
        [Required]
        public int UserID { get; set; }

        [MaxLength(100)]
        public string FirstName { get; set; } = string.Empty;

        [MaxLength(100)]
        public string LastName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email є обов'язковим")]
        [EmailAddress(ErrorMessage = "Неправильний формат Email")]
        [MaxLength(255)]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "ID ролi є обов'язковим")]
        public int RoleID { get; set; }
    }
}