namespace Sanakan.ShindenApi.Utilities
{
    public class UrlHelpers
    {
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

        public static string GetUserAvatarURL(ulong? imageID, ulong userID)
        {
            if (!imageID.HasValue)
            {
                return GetPlaceholderUserImageURL();
            }
            else if (imageID == 0)
            {
                return GetPlaceholderUserImageURL();
            }

            return $"{Constants.ShindenCdnUrl}cdn1/avatars/225x350/{userID}.jpg?v{imageID}";
        }

        public static string GetPersonPictureURL(ulong? imageID) => GetBigImageURL(imageID);

        public static string GetPlaceholderImageURL()
        {
            return $"{Constants.ShindenCdnUrl}cdn1/other/placeholders/title/225x350.jpg";
        }

        public static string GetPlaceholderUserImageURL()
        {
            return $"{Constants.ShindenCdnUrl}cdn1/other/placeholders/user/100x100.jpg";
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