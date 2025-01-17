﻿using Sanakan.DAL.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Sanakan.DAL.Repositories.Abstractions
{
    public interface ICardRepository :
        ICreateRepository<Card>,
        IRemoveRepository<Card>,
        ISaveRepository
    {
        Task<Card?> GetByIdNoTrackingAsync(ulong wid);

        Task<List<Card>> GetCardsWithTagAsync(string tag);

        Task<User?> GetByShindenUserIdAsync(ulong shindenUserId);

        Task<List<Card>> GetByIdsAsync(IEnumerable<ulong> ids);

        Task<List<Card>> GetByIdsAsync(IEnumerable<ulong> ids, CardQueryOptions cardQueryOptions);

        Task<List<Card>> GetByCharacterIdAsync(ulong characterId);

        Task<List<Card>> GetByCharacterIdAsync(ulong characterId, CardQueryOptions cardQueryOptions);

        Task<List<Card>> GetByCharacterIdsAsync(IEnumerable<ulong> characterIds);

        Task<Card?> GetByIdAsync(ulong id);

        Task<Card?> GetByIdAsync(ulong id, CardQueryOptions cardQueryOptions);

        Task<List<Card>> GetByIdFirstOrLastOwnerAsync(ulong discordUserId);

        Task<int> CountByRarityAndSucceedingIdAsync(Rarity rarity, ulong wid);

        Task<List<Card>> GetByCharactersAndNotInUserGameDeckAsync(ulong userId, IEnumerable<ulong> characterIds);

        Task<List<Card>> GetByGameDeckIdAsync(ulong gameDeckId, int offset, int count);

        Task<List<Card>> GetByGameDeckIdAsync(ulong gameDeckId);

        Task<List<ulong>> GetByExcludedGameDeckIdsAsync(IEnumerable<ulong> allUsers);

        Task<List<Card>> GetAsync(ulong gameDeckId, CardsQueryFilter filter);
    }
}
