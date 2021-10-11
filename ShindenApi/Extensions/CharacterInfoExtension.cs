using Shinden.API;
using Shinden.Models;
using Shinden.Models.Entities;
using Shinden.Models.Initializers;
using System;
using System.Collections.Generic;
using System.Web;

namespace Shinden.Extensions
{
    public static class CharacterInfoExtension
    {
        public static ICharacterInfo ToModel(this API.CharacterInfo info)
        {
            ulong.TryParse(info?.CharacterId, out var cID);
            ulong.TryParse(info?.PictureArtifactId, out var pID);

            bool.TryParse(info?.IsReal, out var isReal);

            return new Models.Entities.CharacterInfo(new InitCharacterInfo()
            {
                Id = cID,
                IsReal = isReal,
                PictureId = pID,
                FavsStats = info?.GetFavs(),
                BirthDate = info.GetBirthDate(),
                DeathDate = info.GetDeathDate(),
                Biography = info?.Bio?.ToModel(),
                Points = info?.Points?.ToModel(),
                Pictures = info?.Pictures?.ToModel(),
                Relations = info?.Relations?.ToModel(),
                Age = HttpUtility.HtmlDecode(info?.Age ?? ""),
                Bust = HttpUtility.HtmlDecode(info?.Bust ?? ""),
                Hips = HttpUtility.HtmlDecode(info?.Hips ?? ""),
                Waist = HttpUtility.HtmlDecode(info?.Waist ?? ""),
                Weight = HttpUtility.HtmlDecode(info?.Weight ?? ""),
                Height = HttpUtility.HtmlDecode(info?.Height ?? ""),
                LastName = HttpUtility.HtmlDecode(info?.LastName ?? ""),
                Gender = new Sex().Parse((info?.Gender ?? "").ToLower()),
                FirstName = HttpUtility.HtmlDecode(info?.FirstName ?? ""),
                Bloodtype = HttpUtility.HtmlDecode(info?.Bloodtype ?? ""),
            });
        }

        public static IPicture ToModel(this API.ImagePicture pic)
        {
            ulong.TryParse(pic?.ArtifactId, out var uID);

            bool.TryParse(pic?.IsAccepted, out var acc);
            bool.TryParse(pic?.Is18Plus, out var is18);

            return new Picture(new InitPicture()
            {
                PictureId = uID,
                Is18Plus = is18,
                IsAccepted = acc,
                Title = HttpUtility.HtmlDecode(pic?.Title ?? ""),
                Type = new PictureType().Parse((pic?.ArtifactType ?? "").ToLower()),
            });
        }

        public static List<IPicture> ToModel(this IEnumerable<API.ImagePicture> info)
        {
            var list = new List<IPicture>();
            foreach (var pic in info)
            {
                var rPic = pic.ToModel();
                if (rPic.Type == PictureType.Image)
                    list.Add(rPic);
            }
            return list;
        }

        public static IEditPoints ToModel(this API.PointsForEdit points)
        {
            ulong.TryParse(points?.UserId, out var uID);
            ulong.TryParse(points?.Avatar, out var aID);

            double.TryParse(points?.Points, out var pt);

            return new EditPoints(uID, points?.Name ?? "", pt, aID);
        }

        public static List<IEditPoints> ToModel(this List<API.PointsForEdit> points)
        {
            var list = new List<IEditPoints>();
            if (points != null) foreach(var item in points) list.Add(item?.ToModel());
            return list;
        }

        public static ICharacterFavs GetFavs(this API.CharacterInfo info)
        {
            double.TryParse(info?.FavStats?.AvgPos, out var avg);

            long.TryParse(info?.FavStats?.Fav, out var fav);
            long.TryParse(info?.FavStats?.Unfav, out var unfav);
            long.TryParse(info?.FavStats?.OnePos, out var onepos);
            long.TryParse(info?.FavStats?.Under3Pos, out var u3p);
            long.TryParse(info?.FavStats?.Under10Pos, out var u10p);
            long.TryParse(info?.FavStats?.Under50Pos, out var u50p);

            return new CharacterFavs(new InitCharacterFavs()
            {
                AvgPos = avg,
                FavCnt = fav,
                UnFavCnt = unfav,
                Under3PosCnt = u3p,
                Under10PosCnt = u10p,
                Under50PosCnt = u50p,
                FirstPosCnt = onepos,
            });
        }

        public static DateTime GetBirthDate(this API.CharacterInfo info)
        {
            if (info?.BirthDate == null) return DateTime.MinValue;

            return DateTime.ParseExact(info.BirthDate, "yyyy-MM-dd",
                System.Globalization.CultureInfo.InvariantCulture);
        }

        public static DateTime GetDeathDate(this API.CharacterInfo info)
        {
            if (info?.DeathDate == null) return DateTime.MinValue;

            return DateTime.ParseExact(info.DeathDate, "yyyy-MM-dd",
                System.Globalization.CultureInfo.InvariantCulture);
        }

        public static ICharacterBiography ToModel(this API.CharacterBio bio)
        {
            ulong.TryParse(bio?.CharacterId, out var cID);
            ulong.TryParse(bio?.CharacterBiographyId, out var dID);

            return new CharacterBiography(new InitBiography()
            {
                Id = dID,
                RelatedId = cID,
                Language = new Language().Parse((bio?.Lang ?? "").ToLower()),
                Content = HttpUtility.HtmlDecode(bio?.Biography ?? "")?.RemoveBBCode(),
            });
        }
    }
}
