using Microsoft.EntityFrameworkCore;
using Sanakan.Common;
using Sanakan.DAL.Models;
using Sanakan.DAL.Repositories.Abstractions;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Sanakan.DAL.Repositories
{
    internal class QuestionRepository : BaseRepository<Question>, IQuestionRepository
    {
        private readonly SanakanDbContext _dbContext;
        private readonly ICacheManager _cacheManager;

        public QuestionRepository(
            SanakanDbContext dbContext,
            ICacheManager cacheManager) : base(dbContext)
        {
            _dbContext = dbContext;
            _cacheManager = cacheManager;
        }

        public async Task<Question?> GetByIdAsync(ulong id)
        {
            var entity = await _dbContext.Questions.FindAsync(id);
            return entity;
        }
        public async Task<Question> GetCachedQuestionAsync(ulong id)
        {
            var key = $"quiz-{id}";

            var cached = _cacheManager.Get<Question>(key);

            if (cached != null)
            {
                return cached;
            }

            var result = await _dbContext
                .Questions
                .AsQueryable()
                .Include(x => x.Answers)
                .AsNoTracking()
                .AsSplitQuery()
                .FirstOrDefaultAsync(x => x.Id == id);

            _cacheManager.Add(key, result);

            return result;
        }
        public async Task<List<Question>> GetCachedAllQuestionsAsync()
        {
            var key = $"quiz";

            var cached = _cacheManager.Get<List<Question>>(key);

            if (cached != null)
            {
                return cached;
            }

            var result = await _dbContext
                .Questions
                .AsQueryable()
                .Include(x => x.Answers)
                .AsNoTracking()
                .AsSplitQuery()
                .ToListAsync();

            _cacheManager.Add(key, result);

            return result;
        }
    }
}
