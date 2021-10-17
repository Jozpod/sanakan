using Shinden.API;
using Shinden.Models;
using Shinden.Modules;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Sanakan.ShindenApi
{
    public interface IShindenClient
    {
        Task<Response<List<IRelation>>> GetCharactersAsync(ulong id);
        Task<Response<IUserInfo>> GetAsync(ulong id);
        Task<Response<List<IUserSearch>>> UserAsync(string nick);
        Task<Response<List<INewEpisode>>> GetNewEpisodesAsync();
        Task<Response<IStaffInfo>> GetStaffInfoAsync(ulong id);
        Task<Response<IStaffInfo>> GetStaffInfoAsync(IIndexable id);
        Task<Response<ICharacterInfo>> GetCharacterInfoAsync(ulong id);
        Task<Response<ICharacterInfo>> GetCharacterInfoAsync(IIndexable id);
    }
}
