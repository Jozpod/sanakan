using System.Threading;
using System.Threading.Tasks;

namespace Sanakan.DAL
{
    public interface IDatabaseSeeder
    {
        Task RunAsync();
    }
}
