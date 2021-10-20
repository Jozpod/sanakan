using Sanakan.DAL.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Sanakan.DAL.Repositories.Abstractions
{
    public interface IQuestionRepository : 
        ICreateRepository<Question>, IRemoveRepository<Question>, ISaveRepository
    {
        Task<List<Question>> GetCachedAllQuestionsAsync();
        Task<Question> GetCachedQuestionAsync(ulong id);
        Task<Question?> GetByIdAsync(ulong id);
    }
}
