using System;
using System.ComponentModel.DataAnnotations;

namespace AdminControl.DTO
{
    public class UserBankCardCreateDto
    {
        [Required]
        public string CardHolderName { get; set; } = string.Empty;

        [Required]
        [StringLength(4, MinimumLength = 4, ErrorMessage = "Суфікс має містити 4 цифри")]
        public string CardNumberSuffix { get; set; } = string.Empty;

        [Required]
        public DateTime ExpiryDate { get; set; }

        [Required]
        public string EncryptedCardData { get; set; } = string.Empty;

        [Required]
        public int UserID { get; set; }
    }
}