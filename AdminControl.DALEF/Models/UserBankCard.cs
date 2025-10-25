using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AdminControl.DALEF.Models
{
    [Table("UserBankCards")]
    public class UserBankCard
    {
        [Key]
        public int BankCardID { get; set; }

        [Required]
        public string CardHolderName { get; set; } = string.Empty; 

        [Required]
        [MaxLength(4)]
        public string CardNumberSuffix { get; set; } = string.Empty; 

        public DateTime ExpiryDate { get; set; }

        [Required]
        public string EncryptedCardData { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; }

        public int UserID { get; set; }

        [ForeignKey("UserID")]
        public virtual User? User { get; set; } 
    }
}