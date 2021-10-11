namespace Shinden.API
{
    public class Url
    {
        private const string SHINDEN_URL = "https://shinden.pl/";
        private const string IMAGE_SERVER_URL = "http://cdn.shinden.eu/";

        public static string GetBigImageURL(ulong? imageID)
        {
            if (!imageID.HasValue) return GetPlaceholderImageURL();
            else if (imageID == 0) return GetPlaceholderImageURL();

            return $"{IMAGE_SERVER_URL}cdn1/images/genuine/{imageID}.jpg";
        }

        public static string GetSmallImageURL(ulong? imageID)
        {
            if (!imageID.HasValue) return GetPlaceholderImageURL();
            else if (imageID == 0) return GetPlaceholderImageURL();

            return $"{IMAGE_SERVER_URL}cdn1/images/225x350/{imageID}.jpg";
        }

        public static string GetUserAvatarURL(ulong? imageID, ulong userID)
        {
            if (!imageID.HasValue) return GetPlaceholderUserImageURL();
            else if (imageID == 0) return GetPlaceholderUserImageURL();

            return $"{IMAGE_SERVER_URL}cdn1/avatars/225x350/{userID}.jpg?v{imageID}";
        }

        public static string GetPersonPictureURL(ulong? imageID) => GetBigImageURL(imageID);

        public static string GetPlaceholderImageURL()
        {
            return $"{IMAGE_SERVER_URL}cdn1/other/placeholders/title/225x350.jpg";
        }

        public static string GetPlaceholderUserImageURL()
        {
            return $"{IMAGE_SERVER_URL}cdn1/other/placeholders/user/100x100.jpg";
        }

        public static string GetSeriesURL(ulong seriesID)
        {
            return $"{SHINDEN_URL}titles/{seriesID}";
        }

        public static string GetMangaURL(ulong seriesID)
        {
            return $"{SHINDEN_URL}manga/{seriesID}";
        }

        public static string GetEpisodeURL(ulong seriesID, ulong epID)
        {
            return $"{SHINDEN_URL}epek/{seriesID}/view/{epID}";
        }

        public static string GetChapterURL(ulong seriesID, ulong chapterID)
        {
            return $"{SHINDEN_URL}/manga/{seriesID}/chapter-id/{chapterID}";
        }

        public static string GetProfileURL(ulong id)
        {
            return $"{SHINDEN_URL}user/{id}";
        }

        public static string GetStaffURL(ulong id)
        {
            return $"{SHINDEN_URL}staff/{id}";
        }

        public static string GetCharacterURL(ulong id)
        {
            return $"{SHINDEN_URL}character/{id}";
        }
    }
}