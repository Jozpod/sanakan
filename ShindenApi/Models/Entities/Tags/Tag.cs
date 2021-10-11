using Shinden.Models.Initializers;

namespace Shinden.Models.Entities
{
    public class Tag : ITag
    {
        public Tag(InitTag Init)
        {
            Id = Init.Id;
            Name = Init.Name;
            ParentId = Init.ParentId;
            IsAccepted = Init.IsAccepted;
            NationalName = Init.NationalName;
        }

        public ulong? ParentId { get; }
        public bool? IsAccepted { get; }
        public string NationalName { get; }

        // IIndexable
        public ulong Id { get; }

        // ITag
        private string InternalName;
        public string Name { get => NationalName ?? InternalName; private set => InternalName = value; }
    
        public override string ToString() => Name;
    }
}