using System;
using System.Collections.Generic;
using Shinden.API;
using Shinden.Models.Initializers;

namespace Shinden.Models.Entities
{
    public class StaffInfo : IStaffInfo
    {
        public StaffInfo(InitStaffInfo Init)
        {
            Id = Init.Id;
            Gender = Init.Gender;
            LastName = Init.LastName;
            FirstName = Init.FirstName;
            StaffType = Init.StaffType;
            BirthDate = Init.BirthDate;
            DeathDate = Init.DeathDate;
            PictureId = Init.PictureId;
            Biography = Init.Biography;
            Relations = Init.Relations;
            BirthPlace = Init.BirthPlace;
            Nationality = Init.Nationality;
        }
        
        private ulong? PictureId { get; }

        // IIndexable
        public ulong Id { get; }

        // IStaffInfo
        public Sex Gender { get; }
        public string LastName { get; }
        public string FirstName { get; }
        public string BirthPlace { get; }
        public DateTime BirthDate { get; }
        public DateTime DeathDate { get; }
        public StaffType StaffType { get; }
        public Language Nationality { get; }
        public IStaffBiography Biography { get; }
        public List<IRelation> Relations { get; }
        public string StaffUrl => Url.GetStaffURL(Id);
        public string PictureUrl => Url.GetPersonPictureURL(PictureId);

        public override string ToString() => $"{FirstName} {LastName}";
    }
}
