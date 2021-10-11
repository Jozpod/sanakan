using System.Collections.Generic;
using Shinden.API;
using Shinden.Models;
using Shinden.Models.Entities;
using Shinden.Models.Initializers;

namespace Shinden.Extensions
{
    public static class QuickSearchResultExtension
    {
        public static AnimeQuickSearch ToAnimeModel(this QuickSearchResult r)
        {
            ulong.TryParse(r?.TitleId, out var tID);
            ulong.TryParse(r?.CoverId, out var cID);

            return new AnimeQuickSearch(new InitAnimeQuickSearch()
            {
                AnimeId = tID,
                CoverId = cID,
                Title = r?.Title,
                Type = r.GetAnimeType(),
                Status = r.GetAnimeStatus()
            });
        }

        public static MangaQuickSearch ToMangaModel(this QuickSearchResult r)
        {
            ulong.TryParse(r?.TitleId, out var tID);
            ulong.TryParse(r?.CoverId, out var cID);

            return new MangaQuickSearch(new InitMangaQuickSearch()
            {
                MangaId = tID,
                CoverId = cID,
                Title = r?.Title,
                Type = r.GetMangaType(),
                Status = r.GetMangaStatus()
            });
        }

        public static IQuickSearch ToModel(this QuickSearchResult r, QuickSearchType type)
        {
            switch(type)
            {
                case QuickSearchType.Anime:
                    return r.ToAnimeModel();
                case QuickSearchType.Manga:
                    return r.ToMangaModel();
                default:
                    return null;
            }
        }

        public static List<IQuickSearch> ToModel(this List<QuickSearchResult> rList, QuickSearchType type)
        {
            var list = new List<IQuickSearch>();
            foreach(var item in rList) list.Add(item.ToModel(type));
            return list;
        }

        public static AnimeType GetAnimeType(this QuickSearchResult r)
        {
            return new AnimeType().Parse((r?.Type ?? "").ToLower());
        }

        public static MangaType GetMangaType(this QuickSearchResult r)
        {
            return new MangaType().Parse((r?.Type ?? "").ToLower());
        }

        private static AnimeStatus GetAnimeStatus(this QuickSearchResult r)
        {
            return new AnimeStatus().Parse((r?.TitleStatus ?? "").ToLower());
        }

        private static MangaStatus GetMangaStatus (this QuickSearchResult r)
        {
            return new MangaStatus().Parse((r?.TitleStatus ?? "").ToLower());
        }
    }
}