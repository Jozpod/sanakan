using System.Threading.Tasks;

namespace Sanakan.Web
{
    public interface IRequestBodyReader
    {
        Task<string> GetStringAsync();
    }
}
