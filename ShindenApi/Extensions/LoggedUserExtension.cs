using Shinden.Models;
using Shinden.Models.Entities;
using Shinden.Models.Initializers;
using System;

namespace Shinden.Extensions
{
    public static class LoggedUserExtension
    {
        public static DateTime GetRegisterDate(this API.Logging info)
        {
            if (info?.LoggedUser?.RegisterDate == null) return DateTime.MinValue;

            return DateTime.ParseExact(info.LoggedUser.RegisterDate, "yyyy-MM-dd HH:mm:ss",
                System.Globalization.CultureInfo.InvariantCulture);
        }

        public static DateTime GetLastActiveDate(this API.Logging info)
        {
            if (info?.LoggedUser?.LastActive == null) return DateTime.MinValue;

            return DateTime.ParseExact(info.LoggedUser.LastActive, "yyyy-MM-dd HH:mm:ss",
                System.Globalization.CultureInfo.InvariantCulture);
        }

        public static DateTime GetBirthDate(this API.Logging info)
        {
            if (info?.LoggedUser?.Birthdate == null) return DateTime.MinValue;

            return DateTime.ParseExact(info.LoggedUser.Birthdate, "yyyy-MM-dd",
                System.Globalization.CultureInfo.InvariantCulture);
        }

        public static ILoggedUser ToModel(this API.Logging info, UserAuth auth)
        {
            ulong.TryParse(info?.LoggedUser?.VbId, out var vID);
            ulong.TryParse(info?.LoggedUser?.UserId, out var uID);
            ulong.TryParse(info?.LoggedUser?.Avatar, out var avatar);
            long.TryParse(info?.LoggedUser?.TotalPoints, out var points);

            return new LoggedUser(new InitLoggedUser()
            {
                Id = uID,
                ForumId = vID,
                SkinId = null,
                ListStats = null,
                LogoImgId = null,
                AvatarId = avatar,
                TotalPoints = points,
                Rank = info?.LoggedUser?.Rank,
                Name = info?.LoggedUser?.Name,
                Email = info?.LoggedUser?.Email,
                BirthDate = info.GetBirthDate(),
                AboutMe = info?.LoggedUser?.AboutMe,
                AnimeCSS = info?.LoggedUser?.AnimeCss,
                MangaCSS = info?.LoggedUser?.MangaCss,
                RegisterDate = info.GetRegisterDate(),
                Signature = info?.LoggedUser?.Signature,
                LastTimeActive = info.GetLastActiveDate(),
                Gender = new Sex().Parse((info?.LoggedUser?.Sex ?? "").ToLower()),
                Session = new Session(info.Session.Id, info.Session.Name, info.Hash, auth),
                Status = new UserStatus().Parse((info?.LoggedUser?.Status ?? "").ToLower()),
                PortalLanguage = new Language().Parse((info?.LoggedUser?.PortalLang ?? "").ToLower()),
            });
        }
    }
}
