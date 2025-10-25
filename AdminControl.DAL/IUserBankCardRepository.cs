using AdminControl.DTO;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AdminControl.DAL
{
    public interface IUserBankCardRepository
    {
        Task<IEnumerable<UserBankCardDto>> GetCardsByUserIdAsync(int userId);

        
        Task<UserBankCardDto> AddCardAsync(UserBankCardCreateDto newCard);
        Task DeleteCardAsync(int cardId);
    }
}