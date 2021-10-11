using System;
using System.Collections.Generic;

namespace Shinden.Models
{
    public interface ICharacterInfo : ICharacterInfoShort
    {
        string Age { get; }
        Sex Gender { get; }
        string Hips { get; }
        string Bust { get; }
        bool IsReal { get; }
        string Waist { get; }
        string Height { get; }
        string Weight { get; }
        string Bloodtype { get; }
        DateTime BirthDate { get; }
        DateTime DeathDate { get; }
        List<IPicture> Pictures { get; }
        ICharacterFavs FavsStats { get; }
        List<IEditPoints> Points { get; }
        List<IRelation> Relations { get; }
        ICharacterBiography Biography { get; }
    }
}