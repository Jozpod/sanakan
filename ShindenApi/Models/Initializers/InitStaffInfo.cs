using System;
using System.Collections.Generic;

namespace Shinden.Models.Initializers
{
    public class InitStaffInfo
    {
        public ulong Id { get; set; }
        public Sex Gender { get; set; }
        public string LastName { get; set; }
        public string FirstName { get; set; }
        public ulong? PictureId { get; set; }
        public string BirthPlace { get; set; }
        public DateTime BirthDate { get; set; }
        public DateTime DeathDate { get; set; }
        public StaffType StaffType { get; set; }
        public Language Nationality { get; set; }
        public IStaffBiography Biography { get; set; }
        public List<IRelation> Relations { get; set; }
    }
}