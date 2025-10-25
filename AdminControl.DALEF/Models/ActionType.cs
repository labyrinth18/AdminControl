using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AdminControl.DALEF.Models
{
    [Table("ActionTypes")]
    public class ActionType
    {
        [Key]
        public int ActionTypeID { get; set; }

        [Required]
        [MaxLength(50)]
        public string ActionName { get; set; } = string.Empty;

        public string? ActionDescription { get; set; }
    }
}