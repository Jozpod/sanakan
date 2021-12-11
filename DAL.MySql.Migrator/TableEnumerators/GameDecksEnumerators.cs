using Sanakan.DAL.Models;
using System;

namespace Sanakan.DAL.MySql.Migrator.TableEnumerators
{
    public class GameDecksEnumerators : TableEnumerator<GameDeck>
    {
        public GameDecksEnumerators(IDbConnection connection)
           : base(connection) { }

        public override GameDeck Current
        {
            get
            {
                Uri? backgroundImageUrl = null;
                if(!_reader.IsDBNull(15))
                {
                    Uri.TryCreate(_reader.GetString(15), UriKind.Absolute, out backgroundImageUrl);
                }

                Uri? foregroundImageUrl = null;
                if (!_reader.IsDBNull(16))
                {
                    Uri.TryCreate(_reader.GetString(15), UriKind.Absolute, out foregroundImageUrl);
                }

                return new()
                {
                    Id = _reader.GetUInt64(0),
                    CTCount = _reader.GetInt64(1),
                    FavouriteWaifuId = _reader.GetUInt64(2),
                    Karma = _reader.GetUInt64(3),
                    ItemsDropped = _reader.GetUInt64(4),
                    WishlistIsPrivate = _reader.GetBoolean(5),
                    PVPCoins = _reader.GetInt64(6),
                    DeckPower = _reader.GetDouble(7),
                    PVPWinStreak = _reader.GetInt64(8),
                    GlobalPVPRank = _reader.GetInt64(9),
                    SeasonalPVPRank = _reader.GetInt64(10),
                    MatchMakingRatio = _reader.GetUInt64(11),
                    PVPDailyGamesPlayed = _reader.GetUInt64(12),
                    PVPSeasonBeginDate = _reader.GetDateTime(13),
                    ExchangeConditions = _reader.GetString(14),
                    BackgroundImageUrl = backgroundImageUrl,
                    ForegroundImageUrl = foregroundImageUrl,
                    ForegroundColor = _reader.GetString(17),
                    ForegroundPosition = _reader.GetInt32(18),
                    BackgroundPosition = _reader.GetInt32(19),
                    MaxNumberOfCards = _reader.GetInt64(20),
                    CardsInGalleryCount = _reader.GetInt32(21),
                    UserId = _reader.GetUInt64(22),
                };
            }
        }

        public override string TableName => nameof(SanakanDbContext.GameDecks);
    }
}
