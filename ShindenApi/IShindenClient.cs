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
        Task<Response<List<IPersonSearch>>> CharacterAsync(string name);
        Task<Response<List<ICharacterInfoShort>>> GetFavCharactersAsync(ulong userId);
        Task<Response<List<ulong>>> GetAllCharactersFromAnimeAsync();
        Task<Response<ITitleInfo>> GetInfoAsync(ulong id);
        Task<Response<List<IRelation>>> GetCharactersAsync(ulong id);
        Task<Response<IUserInfo>> GetAsync(ulong id);
        Task<Response<List<IUserSearch>>> SearchUserAsync(string nick);
        Task<Response<List<INewEpisode>>> GetNewEpisodesAsync();
        Task<Response<IStaffInfo>> GetStaffInfoAsync(ulong id);
        Task<Response<IStaffInfo>> GetStaffInfoAsync(IIndexable id);
        Task<Response<ICharacterInfo>> GetCharacterInfoAsync(ulong id);
        Task<Response<ICharacterInfo>> GetCharacterInfoAsync(IIndexable id);
    }
}
