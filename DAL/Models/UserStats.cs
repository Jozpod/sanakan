using System.Text.Json.Serialization;

namespace Sanakan.DAL.Models
{
    /// <summary>
    /// Describes user statistics.
    /// </summary>
    public class UserStats
    {
        public ulong UserId { get; set; }

        public long ScLost { get; set; }

        public long IncomeInSc { get; set; }

        public long SlotMachineGames { get; set; }

        public long Tail { get; set; }

        public long Head { get; set; }

        public long Hit { get; set; }

        public long Misd { get; set; }

        public long RightAnswers { get; set; }

        public long TotalAnswers { get; set; }

        public long TurnamentsWon { get; set; }

        public long UpgradedCardsCount { get; set; }

        public long SacrificedCardsCount { get; set; }

        public long DestroyedCardsCount { get; set; }

        /// <summary>
        /// The number of cards user has unlocked.
        /// </summary>
        public long UnleashedCardsCount { get; set; }

        public long ReleasedCards { get; set; }

        public long OpenedBoosterPacks { get; set; }

        public long OpenedBoosterPacksActivity { get; set; }

        public long YamiUpgrades { get; set; }

        public long RaitoUpgrades { get; set; }

        public long YatoUpgrades { get; set; }

        public long WastedTcOnCookies { get; set; }

        public long WastedTcOnCards { get; set; }

        public long UpgradedToSSS { get; set; }

        public long WastedPuzzlesOnCookies { get; set; }

        public long WastedPuzzlesOnCards { get; set; }

        [JsonIgnore]
        public virtual User? User { get; set; }
    }
}
