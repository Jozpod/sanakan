using System;
using System.Text.Json.Serialization;

namespace Sanakan.ShindenApi.Models
{
    public class Time
    {
        [JsonPropertyName("h")]
        public ulong Hours { get; set; }

        [JsonPropertyName("n")]
        public ulong Months { get; set; }

        [JsonPropertyName("d")]
        public ulong Days { get; set; }

        [JsonPropertyName("m")]
        public ulong Minutes { get; set; }

        [JsonPropertyName("t")]
        public string T { get; set; }

        [JsonPropertyName("y")]
        public ulong Years { get; set; }

        public TimeSpan ToTimeSpan() => new TimeSpan((int)Days, (int)Hours, (int)Minutes, 0, 0);
    }
}
