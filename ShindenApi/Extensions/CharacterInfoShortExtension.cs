using Shinden.API;
using Shinden.Models;
using Shinden.Models.Entities;
using Shinden.Models.Initializers;
using System;
using System.Collections.Generic;
using System.Web;

namespace Shinden.Extensions
{
    public static class CharacterInfoShortExtension
    {
        public static ICharacterInfoShort ToModel(this API.FavCharacter info)
        {
            ulong.TryParse(info?.CharacterId, out var cID);
            ulong.TryParse(info?.PictureArtifactId, out var pID);

            return new Models.Entities.CharacterInfoShort(cID,  HttpUtility.HtmlDecode(info?.LastName ?? ""), HttpUtility.HtmlDecode(info?.FirstName ?? ""), pID);
        }

        public static List<ICharacterInfoShort> ToModel(this List<API.FavCharacter> info)
        {
            var list = new List<ICharacterInfoShort>();
            foreach(var item in info) list.Add(item.ToModel());
            return list;
        }
    }
}