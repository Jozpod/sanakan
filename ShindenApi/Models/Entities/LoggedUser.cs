using Shinden.API;
using Shinden.Models.Initializers;
using System;

namespace Shinden.Models.Entities
{
    public class LoggedUser : ILoggedUser
    {
        public LoggedUser(InitLoggedUser Init)
        {
            Id = Init.Id;
            Name = Init.Name;
            Rank = Init.Rank;
            Email = Init.Email;
            SkinId = Init.SkinId;
            Status = Init.Status;
            Gender = Init.Gender;
            ForumId = Init.ForumId;
            AboutMe = Init.AboutMe;
            Session = Init.Session;
            AvatarId = Init.AvatarId;
            AnimeCSS = Init.AnimeCSS;
            MangaCSS = Init.MangaCSS;
            ListStats = Init.ListStats;
            LogoImgId = Init.LogoImgId;
            Signature = Init.Signature;
            BirthDate = Init.BirthDate;
            TotalPoints = Init.TotalPoints;
            RegisterDate = Init.RegisterDate;
            PortalLanguage = Init.PortalLanguage;
            LastTimeActive = Init.LastTimeActive;
        }
        
        private string Email { get; }
        private ulong? AvatarId { get; }

        // IIndexable && IUserInfo
        public ulong Id { get; }

        // IUserInfo
        public string Name { get; }
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
        public string AvatarUrl => Url.GetUserAvatarURL(AvatarId, Id);

        // ILoggedUser
        public Sex Gender { get; }
        public ISession Session { get; }
        public DateTime BirthDate { get; }

        public override string ToString() => Name;
    }
}
