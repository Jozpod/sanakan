using System;

namespace Shinden.Models.Initializers
{
    public class InitLoggedUser : InitUserInfo
    {
        public Sex Gender { get; set; }
        public ISession Session { get; set; }
        public DateTime BirthDate { get; set; }
    }
}