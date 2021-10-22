using Microsoft.EntityFrameworkCore;
using Sanakan.DAL.Models;
using Sanakan.DAL.Repositories.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sanakan.DAL.Repositories
{
    public class GameDeckRepository : BaseRepository<GameDeck>, IGameDeckRepository
    {
        private readonly BuildDatabaseContext _dbContext;

        public GameDeckRepository(
            BuildDatabaseContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }

        public Task<List<GameDeck>> GetByAnimeIdAsync(ulong id)
        {
            return _dbContext.GameDecks
                .Include(x => x.Wishes)
                .Where(x => !x.WishlistIsPrivate
                    && x.Wishes.Any(c => c.Type == WishlistObjectType.Title
                    && c.ObjectId == id))
                .ToListAsync();
        }

        public Task<List<GameDeck>> GetByCardIdAndCharacterAsync(ulong cardId, ulong character)
        {
            var wishlists = _dbContext.GameDecks
                .Include(x => x.Wishes)
                .AsNoTracking()
                .Where(x => !x.WishlistIsPrivate
                    && (x.Wishes.Any(c => c.Type == WishlistObjectType.Card
                    && c.ObjectId == cardId)
                        || x.Wishes.Any(c => c.Type == WishlistObjectType.Character
                            && c.ObjectId == character)))
                .ToListAsync();

            return wishlists;
        }
    }
}
