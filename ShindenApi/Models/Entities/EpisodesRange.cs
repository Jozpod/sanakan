namespace Shinden.Models.Entities
{
    public class EpisodesRange : IEpisodesRange, IIndexable
    {
        public EpisodesRange(long Min, long Max, ulong Id)
        {
            this.Min = Min;
            this.Max = Max;
            this.Id = Id;
        }

        // IIndexable
        public ulong Id { get; }

        // IEpisodesRange
        public long Min { get; }
        public long Max { get; }

        public override string ToString() => $"{Min}-{Max}";
    }
}
