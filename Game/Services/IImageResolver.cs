using System;
using System.IO;
using System.Threading.Tasks;

namespace Sanakan.Game.Services
{
    public interface IImageResolver
    {
        Task<Stream?> GetAsync(Uri url);
    }
}
