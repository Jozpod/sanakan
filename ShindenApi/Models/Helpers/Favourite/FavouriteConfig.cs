namespace Shinden.Models
{
    public enum FavouriteType
    {
        Character,
        Title,
        Staff,
    }

    public class FavouriteConfig
    {
        public FavouriteType Type { get; set; }
        public ulong FavouriteId { get; set; }
        public ulong UserId { get; set; }
    }
}