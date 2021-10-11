using System.Collections.Generic;
using System.Threading.Tasks;
using Shinden.API;
using Shinden.Extensions;
using Shinden.Models;

namespace Shinden.Modules
{
    public class TitleModule
    {
        private readonly RequestManager _manager;

        public TitleModule(RequestManager manager)
        {
            _manager = manager;
        }

        public async Task<Response<IEpisodesRange>> GetEpisodesRangeAsync(ulong id)
        {
            var raw = await _manager.QueryAsync(new GetTitleEpisodesRangeQuery(id)).ConfigureAwait(false);
            return new ResponseFinal<IEpisodesRange>(raw.Code, raw.Body?.ToModel(id));
        }

        public async Task<Response<IEpisodesRange>> GetEpisodesRangeAsync(IIndexable index)
        {
            return await GetEpisodesRangeAsync(index.Id).ConfigureAwait(false);
        }

        public async Task<Response<List<IEpisode>>> GetEpisodesAsync(ulong id)
        {
            var raw = await _manager.QueryAsync(new GetTitleEpisodesQuery(id)).ConfigureAwait(false);
            return new ResponseFinal<List<IEpisode>>(raw.Code, raw.Body?.ToModel());
        }

        public async Task<Response<List<IEpisode>>> GetEpisodesAsync(IIndexable index)
        {
            return await GetEpisodesAsync(index.Id).ConfigureAwait(false);
        }

        public async Task<Response<ITitleInfo>> GetInfoAsync(ulong id)
        {
            var raw = await _manager.QueryAsync(new GetTitleInfoQuery(id)).ConfigureAwait(false);
            return new ResponseFinal<ITitleInfo>(raw.Code, raw.Body?.ToModel());
        }

        public async Task<Response<ITitleInfo>> GetInfoAsync(IIndexable index)
        {
            return await GetInfoAsync(index.Id).ConfigureAwait(false);
        }

        public async Task<Response<List<IRecommendation>>> GetRecommendationsAsync(ulong id)
        {
            var raw = await _manager.QueryAsync(new GetTitleRecommendationsQuery(id)).ConfigureAwait(false);
            return new ResponseFinal<List<IRecommendation>>(raw.Code, raw.Body?.ToModel());
        }

        public async Task<Response<List<IRecommendation>>> GetRecommendationsAsync(IIndexable index)
        {
            return await GetRecommendationsAsync(index.Id).ConfigureAwait(false);
        }

        public async Task<Response<List<IReview>>> GetReviewsAsync(ulong id)
        {
            var raw = await _manager.QueryAsync(new GetTitleReviewsQuery(id)).ConfigureAwait(false);
            return new ResponseFinal<List<IReview>>(raw.Code, raw.Body?.ToModel(id));
        }

        public async Task<Response<List<IReview>>> GetReviewsAsync(IIndexable index)
        {
            return await GetReviewsAsync(index.Id).ConfigureAwait(false);
        }

        public async Task<Response<List<ITitleRelation>>> GetRelationsAsync(ulong id)
        {
            var raw = await _manager.QueryAsync(new GetTitleRelatedQuery(id)).ConfigureAwait(false);
            return new ResponseFinal<List<ITitleRelation>>(raw.Code, raw.Body?.ToModel());
        }

        public async Task<Response<List<ITitleRelation>>> GetRelationsAsync(IIndexable index)
        {
            return await GetRelationsAsync(index.Id).ConfigureAwait(false);
        }

        public async Task<Response<List<IRelation>>> GetCharactersAsync(ulong id)
        {
            var raw = await _manager.QueryAsync(new GetTitleCharactersQuery(id)).ConfigureAwait(false);
            return new ResponseFinal<List<IRelation>>(raw.Code, raw.Body?.Relations?.ToModel());
        }

        public async Task<Response<List<IRelation>>> GetCharactersAsync(IIndexable index)
        {
            return await GetCharactersAsync(index.Id).ConfigureAwait(false);
        }
    }
}