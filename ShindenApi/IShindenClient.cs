using Shinden.API;
using Shinden.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Sanakan.ShindenApi
{
    public interface IShindenClient
    {
        Task<Response<List<INewEpisode>>> GetNewEpisodesAsync();
        Task<Response<IStaffInfo>> GetStaffInfoAsync(ulong id);
        Task<Response<IStaffInfo>> GetStaffInfoAsync(IIndexable id);
        Task<Response<ICharacterInfo>> GetCharacterInfoAsync(ulong id);
        Task<Response<ICharacterInfo>> GetCharacterInfoAsync(IIndexable id);
    }
}
