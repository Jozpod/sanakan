using Shinden.API;
using Shinden.Models.Initializers;

namespace Shinden.Models.Entities
{
    public class SimpleUser : ISimpleUser
    {
        public SimpleUser(ulong Id, string Name, ulong? AvatarId)
        {
            this.Id = Id;
            this.Name = Name;
            this.AvatarId = AvatarId;
        }
        
        private ulong? AvatarId { get; }

        // IIndexable
        public ulong Id { get; }

        // ISimpleUser
        public string Name { get; }
        public string AvatarUrl => Url.GetUserAvatarURL(AvatarId, Id);

        public override string ToString() => Name;
    }
}
