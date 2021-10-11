using System;

namespace Shinden.Models.Entities
{
    public class DateTimePrecision : IDateTimePrecision
    {
        public DateTimePrecision(DateTime Date, ulong? Precision)
        {
            this.Date = Date;
            this.Precision = Precision;
        }
        
        private DateTime DefaultDate => new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);

        // IDateTimePrecision
        public DateTime Date { get; }
        public ulong? Precision { get; }
        public bool IsSpecified => DefaultDate.Date != Date.Date;


        public override string ToString()
        {
            if (!Precision.HasValue) return Date.ToString("dd.MM.yyyy");

            switch (Precision)
            {
                case 1: return Date.ToString("yyyy");
                case 2: return Date.ToString("MM.yyyy");
                default: return Date.ToString("dd.MM.yyyy");
            }
        }
    }
}
