using Shinden.API;
using Shinden.Models.Initializers;
using System;

namespace Shinden.Models.Entities
{
    public class UserInfo : IUserInfo
    {
        public UserInfo(InitUserInfo Init)
        {
            Id = Init.Id;
            Name = Init.Name;
            Rank = Init.Rank;
            Email = Init.Email;
            SkinId = Init.SkinId;
            Status = Init.Status;
            ForumId = Init.ForumId;
            AboutMe = Init.AboutMe;
            AvatarId = Init.AvatarId;
            AnimeCSS = Init.AnimeCSS;
            MangaCSS = Init.MangaCSS;
            ListStats = Init.ListStats;
            LogoImgId = Init.LogoImgId;
            Signature = Init.Signature;
            TotalPoints = Init.TotalPoints;
            RegisterDate = Init.RegisterDate;
            PortalLanguage = Init.PortalLanguage;
            LastTimeActive = Init.LastTimeActive;
        }
        
        private string Email { get; }
        private ulong? AvatarId { get; }

        // IIndexable
        public ulong Id { get; }
        
        // ISimpleUser
        public string Name { get; }
        public string AvatarUrl => Url.GetUserAvatarURL(AvatarId, Id);

        // IUserInfo
        public string Rank { get; }
        public ulong? SkinId { get; }
        public ulong? ForumId { get; }
        public string AboutMe { get; }
        public string AnimeCSS { get; }
        public string MangaCSS { get; }
        public ulong? LogoImgId { get; }
        public string Signature { get; }
        public long? TotalPoints { get; }
        public UserStatus Status { get; }
        public IListStats ListStats { get; }
        public DateTime RegisterDate { get; }
        public Language PortalLanguage { get; }
        public DateTime LastTimeActive { get; }

        public override string ToString() => Name;
    }
}
