using System;
using System.Collections.Generic;

namespace Shinden.Models
{
    public interface IStaffInfo : IIndexable
    {
        Sex Gender { get; }
        string StaffUrl { get; }
        string LastName { get; }
        string FirstName { get; }
        string BirthPlace { get; }
        string PictureUrl { get; }
        DateTime BirthDate { get; }
        DateTime DeathDate { get; }
        StaffType StaffType { get; }
        Language Nationality { get; }
        IStaffBiography Biography { get; }
        List<IRelation> Relations { get; }
    }
}