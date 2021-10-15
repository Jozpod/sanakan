using Microsoft.Extensions.Logging;
using Shinden.API;
using Shinden.Extensions;
using Shinden.Models;
using Shinden.Modules;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Sanakan.ShindenApi
{
    public class ShindenClient : IShindenClient
    {
        private readonly RequestManager _manager;
        private readonly ILogger<ShindenClient> _logger;

        public TitleModule Title { get; }
        public SearchModule Search { get; }
        public ExperimentalModule Ex { get; }
        public LoggedInUserModule.UserModule User { get; }

        public ShindenClient(Auth authenticator, ILogger<ShindenClient> logger)
        {
            //_logger.LogInformation($"Runing as: {authenticator.GetUserAgent()}");
            _manager = new RequestManager(authenticator, logger);

            Title = new TitleModule(_manager);
            Search = new SearchModule(_manager);
            Ex = new ExperimentalModule(_manager);
            User = new LoggedInUserModule.UserModule(_manager, _logger);
        }

        public async Task<Response<List<INewEpisode>>> GetNewEpisodesAsync()
        {
            var raw = await _manager.QueryAsync(new GetNewEpisodesQuery()).ConfigureAwait(false);
            return new ResponseFinal<List<INewEpisode>>(raw.Code, new List<INewEpisode>(raw.Body?.ToModel()));
        }

        public async Task<Response<IStaffInfo>> GetStaffInfoAsync(ulong id)
        {
            var raw = await _manager.QueryAsync(new GetStaffInfoQuery(id)).ConfigureAwait(false);
            return new ResponseFinal<IStaffInfo>(raw.Code, raw.Body?.ToModel());
        }

        public async Task<Response<IStaffInfo>> GetStaffInfoAsync(IIndexable id)
        {
            return await GetStaffInfoAsync(id.Id).ConfigureAwait(false);
        }

        public async Task<Response<ICharacterInfo>> GetCharacterInfoAsync(ulong id)
        {
            var raw = await _manager.QueryAsync(new GetCharacterfInfoQuery(id)).ConfigureAwait(false);
            return new ResponseFinal<ICharacterInfo>(raw.Code, raw.Body?.ToModel());
        }

        public async Task<Response<ICharacterInfo>> GetCharacterInfoAsync(IIndexable id)
        {
            return await GetCharacterInfoAsync(id.Id).ConfigureAwait(false);
        }
    }
}