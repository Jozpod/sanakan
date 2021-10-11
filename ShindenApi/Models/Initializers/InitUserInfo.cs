using System;

namespace Shinden.Models.Initializers
{
    public class InitUserInfo
    {
        public ulong Id { get; set; }
        public string Name { get; set; }
        public string Rank { get; set; }
        public string Email { get; set; }
        public ulong? SkinId { get; set; }
        public ulong? ForumId { get; set; }
        public string AboutMe { get; set; }
        public ulong? AvatarId { get; set; }
        public string AnimeCSS { get; set; }
        public string MangaCSS { get; set; }
        public ulong? LogoImgId { get; set; }
        public string Signature { get; set; }
        public long? TotalPoints { get; set; }
        public UserStatus Status { get; set; }
        public IListStats ListStats { get; set; }
        public DateTime RegisterDate { get; set; }
        public Language PortalLanguage { get; set; }
        public DateTime LastTimeActive { get; set; }
    }
}