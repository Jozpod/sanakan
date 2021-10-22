using Microsoft.Extensions.Options;
using System;
using System.Threading.Tasks;

namespace Sanakan.Common
{
    public interface IWritableOptions<out T> : IOptions<T> where T : class, new()
    {
        Task UpdateAsync(Action<T> applyChanges);
    }
}
