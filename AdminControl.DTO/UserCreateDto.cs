using System.ComponentModel.DataAnnotations;

namespace AdminControl.DTO
{
    public class UserCreateDto
    {
        [Required(ErrorMessage = "Логiн є обов'язковим")]
        [MaxLength(50)]
        public string Login { get; set; } = string.Empty;

        [Required(ErrorMessage = "Пароль є обов'язковим")]
        public string Password { get; set; } = string.Empty;

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