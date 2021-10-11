using System;

namespace Shinden.Models
{
    public interface ILoggedUser : IUserInfo
    {
        Sex Gender { get; }
        ISession Session { get; }
        DateTime BirthDate { get; }
    }
}