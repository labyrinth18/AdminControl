using System.ComponentModel.DataAnnotations;

namespace AdminControl.DTO
{
    public class UserUpdateDto
    {
        [Required]
        public int UserID { get; set; }

        [Required(ErrorMessage = "Ім'я є обов'язковим")]
        [MinLength(2, ErrorMessage = "Ім'я повинно містити мінімум 2 символи")]
        [MaxLength(100, ErrorMessage = "Ім'я не може перевищувати 100 символів")]
        public string FirstName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Прізвище є обов'язковим")]
        [MinLength(2, ErrorMessage = "Прізвище повинно містити мінімум 2 символи")]
        [MaxLength(100, ErrorMessage = "Прізвище не може перевищувати 100 символів")]
        public string LastName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email є обов'язковим")]
        [EmailAddress(ErrorMessage = "Невірний формат Email")]
        [MaxLength(255, ErrorMessage = "Email не може перевищувати 255 символів")]
        public string Email { get; set; } = string.Empty;

        [Phone(ErrorMessage = "Невірний формат номера телефону")]
        [MaxLength(20, ErrorMessage = "Номер телефону не може перевищувати 20 символів")]
        public string? PhoneNumber { get; set; }

        [MaxLength(500, ErrorMessage = "Адреса не може перевищувати 500 символів")]
        public string? Address { get; set; }

        [MaxLength(10)]
        public string? Gender { get; set; }

        [Required(ErrorMessage = "Роль є обов'язковою")]
        public int RoleID { get; set; }

        public bool IsActive { get; set; }
    }
}
