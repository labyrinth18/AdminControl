using System.ComponentModel.DataAnnotations;

namespace AdminControl.DTO
{
    public class RoleUpdateDto
    {
        [Required]
        public int RoleID { get; set; }

        [Required(ErrorMessage = "Назва ролi є обов'язковою")]
        [MaxLength(50, ErrorMessage = "Назва ролi не може перевищувати 50 символiв")]
        public string RoleName { get; set; } = string.Empty;
    }
}