using Sanakan.ShindenApi.Utilities;
using Shinden.API;

namespace Shinden.Models.Entities
{
    public class EditPoints : IEditPoints
    {
        public EditPoints(ulong Id, string Name, double Points, ulong? AvatarId)
        {
            this.Id = Id;
            this.Name = Name;
            this.Points = Points;
            this.AvatarId = AvatarId;
        }
        
        private ulong? AvatarId { get; }

        // IIndexable
        public ulong Id { get; }

        // IEditPoints
        public string Name { get; }
        public double Points { get; }
        public string AvatarUrl => UrlHelpers.GetUserAvatarURL(AvatarId, Id);

        public override string ToString() => $"{Name} - {Points}";
    }
}
