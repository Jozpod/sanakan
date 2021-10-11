using Shinden.API;
using Shinden.Models;
using Shinden.Models.Entities;
using Shinden.Models.Initializers;
using System;
using System.Collections.Generic;
using System.Web;

namespace Shinden.Extensions
{
    public static class StaffInfoExtension
    {
        public static IStaffInfo ToModel(this API.StaffInfo info)
        {
            ulong.TryParse(info?.StaffId, out var sID);
            ulong.TryParse(info?.PictureArtifactId, out var pID);

            return new Models.Entities.StaffInfo(new InitStaffInfo()
            {
                Id = sID,
                PictureId = pID,
                Biography = info?.Bio?.ToModel(),
                BirthDate = info.GetBirthDate(),
                DeathDate = info.GetDeathDate(),
                Relations = info?.Relations?.ToModel(),
                LastName = HttpUtility.HtmlDecode(info.LastName),
                FirstName = HttpUtility.HtmlDecode(info.FirstName),
                BirthPlace = HttpUtility.HtmlDecode(info.BirthPlace),
                Gender = new Sex().Parse((info?.Gender ?? "").ToLower()),
                StaffType = new StaffType().Parse((info?.StaffType ?? "").ToLower()),
                Nationality = new Language().Parse((info?.Nationality ?? "").ToLower()),
            });
        }

        public static IRelation ToModel(this API.Relation relation)
        {
            ulong.TryParse(relation?.ManyId, out var id);
            ulong.TryParse(relation?.StaffId, out var sID);
            ulong.TryParse(relation?.CharacterId, out var cID);

            return new Models.Entities.Relation(new InitRelation()
            {
                Language = new Language().Parse((relation?.SeiyuuLang ?? "").ToLower()),
                CharacterFirstName = HttpUtility.HtmlDecode(relation?.FirstName ?? ""),
                CharacterLastName = HttpUtility.HtmlDecode(relation?.LastName ?? ""),
                StaffFirstName = HttpUtility.HtmlDecode(relation?.SFirstName ?? ""),
                StaffLastName = HttpUtility.HtmlDecode(relation?.SLastName ?? ""),
                Title = HttpUtility.HtmlDecode(relation?.Title ?? ""),
                StaffPosition = relation?.Position,
                CharacterRole = relation?.Role,
                CharacterId = cID,
                StaffId = sID,
                Id = id,
            });
        }

        public static List<IRelation> ToModel(this List<API.Relation> relations)
        {
            var list = new List<IRelation>();
            if (relations != null) foreach(var rel in relations) list.Add(rel?.ToModel());
            return list;
        }

        public static DateTime GetBirthDate(this API.StaffInfo info)
        {
            if (info?.BirthDate == null) return DateTime.MinValue;

            return DateTime.ParseExact(info.BirthDate, "yyyy-MM-dd",
                System.Globalization.CultureInfo.InvariantCulture);
        }

        public static DateTime GetDeathDate(this API.StaffInfo info)
        {
            if (info?.DeathDate == null) return DateTime.MinValue;

            return DateTime.ParseExact(info.DeathDate, "yyyy-MM-dd",
                System.Globalization.CultureInfo.InvariantCulture);
        }

        public static IStaffBiography ToModel(this API.StaffBio bio)
        {
            ulong.TryParse(bio?.StaffId, out var sID);
            ulong.TryParse(bio?.StaffBiographyId, out var dID);

            return new StaffBiography(new InitBiography()
            {
                Id = dID,
                RelatedId = sID,
                Language = new Language().Parse((bio?.Lang ?? "").ToLower()),
                Content = HttpUtility.HtmlDecode(bio?.Biography ?? "")?.RemoveBBCode(),
            });
        }
    }
}
