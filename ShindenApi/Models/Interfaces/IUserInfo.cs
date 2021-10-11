using System;

namespace Shinden.Models
{
    public interface IUserInfo : ISimpleUser
    {
        string Rank { get; }
        ulong? SkinId { get; }
        ulong? ForumId { get; }
        string AboutMe { get; }
        string AnimeCSS { get; }
        string MangaCSS { get; }
        ulong? LogoImgId { get; }
        string Signature { get; }
        long? TotalPoints { get; }
        UserStatus Status { get; }
        IListStats ListStats { get; }
        DateTime RegisterDate { get; }
        Language PortalLanguage { get; }
        DateTime LastTimeActive { get; }
    }
}