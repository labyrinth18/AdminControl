using System; 
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AdminControl.DALEF.Models
{
    [Table("AdminActionLog")]
    public class AdminActionLog
    {
        [Key]
        public int LogID { get; set; }

        public int AdminUserID { get; set; } 

        public int? TargetUserID { get; set; } 

        public int ActionTypeID { get; set; } 

        [MaxLength(255)]
        public string? Reason { get; set; } 

        public DateTime ActionDate { get; set; }

        [ForeignKey("AdminUserID")]
        public virtual User? AdminUser { get; set; }

        [ForeignKey("TargetUserID")]
        public virtual User? TargetUser { get; set; }

        [ForeignKey("ActionTypeID")]
        public virtual ActionType? ActionType { get; set; }
    }
}