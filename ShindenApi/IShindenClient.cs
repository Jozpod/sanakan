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
        TitleModule Title { get; }
        SearchModule Search { get; }
        ExperimentalModule Ex { get; }
        LoggedInUserModule.UserModule User { get; }
        Task<Response<List<INewEpisode>>> GetNewEpisodesAsync();
        Task<Response<IStaffInfo>> GetStaffInfoAsync(ulong id);
        Task<Response<IStaffInfo>> GetStaffInfoAsync(IIndexable id);
        Task<Response<ICharacterInfo>> GetCharacterInfoAsync(ulong id);
        Task<Response<ICharacterInfo>> GetCharacterInfoAsync(IIndexable id);
    }
}
