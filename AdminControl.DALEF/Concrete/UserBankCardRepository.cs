using AdminControl.DAL;
using AdminControl.DALEF.Models;
using AdminControl.DTO;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AdminControl.DALEF.Concrete
{
    public class UserBankCardRepository : IUserBankCardRepository
    {
        private readonly AdminControlContext _context;

        public UserBankCardRepository(AdminControlContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<UserBankCardDto>> GetCardsByUserIdAsync(int userId)
        {
            return await _context.UserBankCards
                .Where(c => c.UserID == userId)

                .Select(c => new UserBankCardDto
                {
                    BankCardID = c.BankCardID,
                    CardHolderName = c.CardHolderName,
                    CardNumberSuffix = c.CardNumberSuffix, 
                    ExpiryDate = c.ExpiryDate,
                    UserID = c.UserID
                })
                .ToListAsync();
        }

        public async Task<UserBankCardDto> AddCardAsync(UserBankCardCreateDto newCardDto)
        {
            var newCard = new UserBankCard
            {
                CardHolderName = newCardDto.CardHolderName,
                CardNumberSuffix = newCardDto.CardNumberSuffix,
                ExpiryDate = newCardDto.ExpiryDate,
                EncryptedCardData = newCardDto.EncryptedCardData,
                UserID = newCardDto.UserID,
                CreatedAt = DateTime.UtcNow
            };

            _context.UserBankCards.Add(newCard);
            await _context.SaveChangesAsync();

            return new UserBankCardDto
            {
                BankCardID = newCard.BankCardID,
                CardHolderName = newCard.CardHolderName,
                CardNumberSuffix = newCard.CardNumberSuffix,
                ExpiryDate = newCard.ExpiryDate,
                UserID = newCard.UserID
            };
        }

        public async Task DeleteCardAsync(int cardId)
        {
            var cardToDelete = await _context.UserBankCards.FindAsync(cardId);
            if (cardToDelete != null)
            {
                _context.UserBankCards.Remove(cardToDelete);
                await _context.SaveChangesAsync();
            }
        }
    }
}