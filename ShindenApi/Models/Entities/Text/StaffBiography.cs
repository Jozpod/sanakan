using Shinden.Models.Initializers;

namespace Shinden.Models.Entities
{
    public class StaffBiography : IStaffBiography
    {
        public StaffBiography(InitBiography Init)
        {
            Id = Init.Id;
            Content = Init.Content;
            StaffId = Init.RelatedId;
            Language = Init.Language;
        }

        // IIndexable
        public ulong Id { get; }

        // IStaffBiography
        public ulong StaffId { get; }
        public string Content { get; }
        public Language Language { get; }

        public override string ToString() => Content;
    }
}