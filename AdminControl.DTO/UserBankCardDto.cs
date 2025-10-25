using System;

namespace AdminControl.DTO
{
    public class UserBankCardDto
    {
        public int BankCardID { get; set; }
        public string CardHolderName { get; set; }
        public string CardNumberSuffix { get; set; }
        public DateTime ExpiryDate { get; set; }
        public int UserID { get; set; }
    }
}