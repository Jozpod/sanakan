namespace Shinden.Models.Entities
{
    public class MeanScore : IMeanScore
    {
        public MeanScore(ulong? Count, double? Rating)
        {
            this.Count = Count;
            this.Rating = Rating;
        }

        // IMeanScore
        public ulong? Count { get; }
        public double? Rating { get; }

        public override string ToString() => Rating.HasValue ? Rating.Value.ToString("0.0") : "0";
    }
}
