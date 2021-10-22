using System;
using System.Collections.Generic;
using Sanakan.ShindenApi.Utilities;
using Shinden.API;
using Shinden.Models.Initializers;

namespace Shinden.Models.Entities
{
    public class CharacterInfo : ICharacterInfo
    {
        public CharacterInfo(InitCharacterInfo Init)
        {
            Id = Init.Id;
            Age = Init.Age;
            Hips = Init.Hips;
            Bust = Init.Bust;
            Waist = Init.Waist;
            IsReal = Init.IsReal;
            Height = Init.Height;
            Weight = Init.Weight;
            Points = Init.Points;
            Gender = Init.Gender;
            Pictures = Init.Pictures;
            LastName = Init.LastName;
            Bloodtype = Init.Bloodtype;
            FirstName = Init.FirstName;
            BirthDate = Init.BirthDate;
            DeathDate = Init.DeathDate;
            PictureId = Init.PictureId;
            Biography = Init.Biography;
            Relations = Init.Relations;
            FavsStats = Init.FavsStats;
        }

        private ulong? PictureId { get; }

        // IIndexable
        public ulong Id { get; }

        // ICharacterInfo
        public string Age { get; }
        public Sex Gender { get; }
        public string Hips { get; }
        public string Bust { get; }
        public bool IsReal { get; }
        public string Waist { get; }
        public string Height { get; }
        public string Weight { get; }
        public string LastName { get; }
        public string FirstName { get; }
        public string Bloodtype { get; }
        public DateTime BirthDate { get; }
        public DateTime DeathDate { get; }
        public List<IPicture> Pictures { get; }
        public ICharacterFavs FavsStats { get; }
        public List<IEditPoints> Points { get; }
        public List<IRelation> Relations { get; }
        public ICharacterBiography Biography { get; }
        public string CharacterUrl => UrlHelpers.GetCharacterURL(Id);
        public string PictureUrl => UrlHelpers.GetPersonPictureURL(PictureId);
        public bool HasImage => PictureUrl != UrlHelpers.GetPlaceholderImageURL();

        public override string ToString() => $"{FirstName} {LastName}";
    }
}
