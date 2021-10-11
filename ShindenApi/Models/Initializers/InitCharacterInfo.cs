using System;
using System.Collections.Generic;

namespace Shinden.Models.Initializers
{
    public class InitCharacterInfo
    {
        public ulong Id { get; set; }
        public Sex Gender { get; set; }
        public string Age { get; set; }
        public string Hips { get; set; }
        public string Bust { get; set; }
        public bool IsReal { get; set; }
        public string Waist { get; set; }
        public string Height { get; set; }
        public string Weight { get; set; }
        public string LastName { get; set; }
        public string FirstName { get; set; }
        public string Bloodtype { get; set; }
        public ulong? PictureId { get; set; }
        public DateTime BirthDate { get; set; }
        public DateTime DeathDate { get; set; }
        public List<IPicture> Pictures { get; set; }
        public List<IEditPoints> Points { get; set; }
        public ICharacterFavs FavsStats { get; set; }
        public List<IRelation> Relations { get; set; }
        public ICharacterBiography Biography { get; set; }
    }
}