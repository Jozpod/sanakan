using Sanakan.ShindenApi.Utilities;
using Shinden.API;
using Shinden.Models.Initializers;

namespace Shinden.Models.Entities
{
    public class UserSearch : IUserSearch
    {
        public UserSearch(InitUserSearch Init)
        {
            Id = Init.Id;
            Name = Init.Name;
            Rank = Init.Rank;
            AvatarId = Init.AvatarId;
        }
        
        private ulong? AvatarId { get; }

        // IIndexable
        public ulong Id { get; }

        // ISimpleUser
        public string Name { get; }
        public string AvatarUrl => UrlHelpers.GetUserAvatarURL(AvatarId, Id);

        // IUserSearch
        public string Rank { get; }

        public override string ToString() => Name;
    }
}
