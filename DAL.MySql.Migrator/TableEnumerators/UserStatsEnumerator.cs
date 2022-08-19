using Sanakan.DAL.Models;

namespace Sanakan.DAL.MySql.Migrator.TableEnumerators
{
    public class UserStatsEnumerator : TableEnumerator<UserStats>
    {
        public UserStatsEnumerator(IDbConnection connection)
            : base(connection)
        {
        }

        public override UserStats Current => new()
        {
            ScLost = _reader.GetInt64(1),
            IncomeInSc = _reader.GetInt64(2),
            SlotMachineGames = _reader.GetInt64(3),
            Tail = _reader.GetInt64(4),
            Head = _reader.GetInt64(5),
            Hit = _reader.GetInt64(6),
            Misd = _reader.GetInt64(7),
            RightAnswers = _reader.GetInt64(8),
            TotalAnswers = _reader.GetInt64(9),
            TournamentsWon = _reader.GetInt64(10),
            UpgradedCardsCount = _reader.GetInt64(11),
            SacrificedCardsCount = _reader.GetInt64(12),
            DestroyedCardsCount = _reader.GetInt64(13),
            UnleashedCardsCount = _reader.GetInt64(14),
            ReleasedCards = _reader.GetInt64(15),
            OpenedBoosterPacks = _reader.GetInt64(16),
            OpenedBoosterPacksActivity = _reader.GetInt64(17),
            YamiUpgrades = _reader.GetInt64(18),
            RaitoUpgrades = _reader.GetInt64(19),
            YatoUpgrades = _reader.GetInt64(20),
            WastedTcOnCookies = _reader.GetInt64(21),
            WastedTcOnCards = _reader.GetInt64(22),
            UpgradedToSSS = _reader.GetInt64(23),
            WastedPuzzlesOnCookies = _reader.GetInt64(24),
            WastedPuzzlesOnCards = _reader.GetInt64(25),
            UserId = _reader.GetUInt64(26),
        };

        public override string TableName => nameof(SanakanDbContext.UsersStats);
    }
}
