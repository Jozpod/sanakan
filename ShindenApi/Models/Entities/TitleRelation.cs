namespace Shinden.Models.Entities
{
    public class TitleRelation : ITitleRelation
    {
        public TitleRelation(ulong Id, string Title, string Type)
        {
            this.Id = Id;
            this.Title = Title;
            this.RelationType = Type;
        }

        // IIndexable
        public ulong Id { get; }

        // ITitleRelation
        public string RelationType { get; }
        public string Title { get; }

        public override string ToString() => Title;
    }
}
