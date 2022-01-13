using System;

namespace Sanakan.ShindenApi.Utilities
{
    public class UrlHelpers
    {
        public static UrlParsingError ParseUrlToShindenId(Uri url, out ulong shindenId)
        {
            shindenId = 0;
            var splited = url.ToString().Split('/');
            bool http = splited[0].Equals("https:") || splited[0].Equals("http:");
            int toChek = http ? 2 : 0;

            if (splited.Length < (toChek == 2 ? 5 : 3))
            {
                return UrlParsingError.InvalidUrl;
            }

            if (splited[toChek].Equals("shinden.pl") || splited[toChek].Equals("www.shinden.pl"))
            {
                if (splited[++toChek].Equals("user") || splited[toChek].Equals("animelist") || splited[toChek].Equals("mangalist"))
                {
                    var data = splited[++toChek].Split('-');
                    if (ulong.TryParse(data[0], out shindenId))
                    {
                        return UrlParsingError.None;
                    }
                }
            }

            if (splited[toChek].Equals("forum.shinden.pl")
                || splited[toChek].Equals("www.forum.shinden.pl"))
            {
                return UrlParsingError.InvalidUrlForum;
            }

            return UrlParsingError.InvalidUrl;
        }

        public static string GetBigImageURL(ulong? imageID)
        {
            if (!imageID.HasValue)
            {
                return GetPlaceholderImageURL();
            }
            else if (imageID == 0)
            {
                return GetPlaceholderImageURL();
            }

            return $"{Constants.ShindenCdnUrl}cdn1/images/genuine/{imageID}.jpg";
        }

        public static string GetSmallImageURL(ulong? imageID)
        {
            if (!imageID.HasValue)
            {
                return GetPlaceholderImageURL();
            }
            else if (imageID == 0)
            {
                return GetPlaceholderImageURL();
            }

            return $"{Constants.ShindenCdnUrl}cdn1/images/225x350/{imageID}.jpg";
        }

        public static Uri GetUserAvatarURL(ulong? imageID, ulong userID)
        {
            if (!imageID.HasValue)
            {
                return GetPlaceholderUserImageURL();
            }
            else if (imageID == 0)
            {
                return GetPlaceholderUserImageURL();
            }

            var uri = new Uri($"{Constants.ShindenCdnUrl}cdn1/avatars/225x350/{userID}.jpg?v{imageID}");

            return uri;
        }

        public static string GetPersonPictureURL(ulong? imageID) => GetBigImageURL(imageID);

        public static string GetPlaceholderImageURL()
        {
            return $"{Constants.ShindenCdnUrl}cdn1/other/placeholders/title/225x350.jpg";
        }

        public static Uri GetPlaceholderUserImageURL()
        {
            return new Uri($"{Constants.ShindenCdnUrl}cdn1/other/placeholders/user/100x100.jpg");
        }

        public static string GetSeriesURL(ulong seriesID)
        {
            return $"{Constants.ShindenUrl}titles/{seriesID}";
        }

        public static string GetMangaURL(ulong seriesID)
        {
            return $"{Constants.ShindenUrl}manga/{seriesID}";
        }

        public static string GetEpisodeURL(ulong seriesID, ulong epID)
        {
            return $"{Constants.ShindenUrl}epek/{seriesID}/view/{epID}";
        }

        public static string GetChapterURL(ulong seriesID, ulong chapterID)
        {
            return $"{Constants.ShindenUrl}/manga/{seriesID}/chapter-id/{chapterID}";
        }

        public static string GetProfileURL(ulong id)
        {
            return $"{Constants.ShindenUrl}user/{id}";
        }

        public static string GetStaffURL(ulong id)
        {
            return $"{Constants.ShindenUrl}staff/{id}";
        }

        public static string GetCharacterURL(ulong id)
        {
            return $"{Constants.ShindenUrl}character/{id}";
        }
    }
}