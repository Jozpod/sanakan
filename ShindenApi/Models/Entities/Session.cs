using System;

namespace Shinden.Models.Entities
{
    public class Session : ISession
    {
        public Session(string Id, string Name, string Hash, UserAuth Auth)
        {
            this.Id = Id;
            this.Name = Name;
            this.Hash = Hash;
            this.Auth = Auth;
            Created = DateTime.Now;
        }

        private readonly UserAuth Auth;
        private DateTime Created { get; set; }

        // ISession
        public string Id { get; private set; }
        public string Name { get; private set; }
        public string Hash { get; private set; }
        public DateTime Expires => Created.AddMinutes(90);

        public bool IsValid() => (_systemClock.UtcNow - Created).Minutes < 90;
        public UserAuth GetAuth() => Auth;

        public void Renew(ISession session)
        {
            Id = session.Id;
            Name = session.Name;
            Hash = session.Hash;
            Created = DateTime.Now;
        }
    }
}