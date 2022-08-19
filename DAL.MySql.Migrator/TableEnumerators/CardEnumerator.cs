using Sanakan.DAL.Models;
using System;

namespace Sanakan.DAL.MySql.Migrator.TableEnumerators
{
    public class CardEnumerator : TableEnumerator<Card>
    {
        public CardEnumerator(IDbConnection connection)
           : base(connection)
        {
        }

        public override Card Current
        {
            get
            {
                Uri.TryCreate(_reader.GetString(19), UriKind.Absolute, out var imageUrl);
                Uri.TryCreate(_reader.GetString(20), UriKind.Absolute, out var customImageUrl);
                Uri.TryCreate(_reader.GetString(25), UriKind.Absolute, out var customBorderUrl);

                return new()
                {
                    Id = _reader.GetUInt64(0),
                    Active = _reader.GetBoolean(1),
                    InCage = _reader.GetBoolean(2),
                    IsTradable = _reader.GetBoolean(3),
                    ExperienceCount = _reader.GetUInt64(4),
                    Affection = _reader.GetUInt64(5),
                    UpgradesCount = _reader.GetInt32(6),
                    RestartCount = _reader.GetInt32(7),
                    Rarity = (Rarity)_reader.GetInt32(8),
                    RarityOnStart = (Rarity)_reader.GetInt32(9),
                    Dere = (Dere)_reader.GetInt32(10),
                    Defence = _reader.GetInt32(11),
                    Attack = _reader.GetInt32(12),
                    Health = _reader.GetInt32(13),
                    Name = _reader.GetString(14),
                    CharacterId = _reader.GetUInt64(15),
                    CreatedOn = _reader.GetDateTime(16),
                    Source = (CardSource)_reader.GetUInt64(17),
                    Title = _reader.GetString(18),
                    ImageUrl = imageUrl,
                    CustomImageUrl = customImageUrl,
                    FirstOwnerId = _reader.GetUInt64(21),
                    LastOwnerId = _reader.GetUInt64(22),
                    IsUnique = _reader.GetBoolean(23),
                    StarStyle = (StarStyle)_reader.GetUInt64(24),
                    CustomBorderUrl = customBorderUrl,
                    MarketValue = _reader.GetUInt64(26),
                    Curse = (CardCurse)_reader.GetInt32(27),
                    CardPower = _reader.GetUInt64(28),
                    EnhanceCount = _reader.GetInt32(29),
                    FromFigure = _reader.GetBoolean(30),
                    Quality = (Quality)_reader.GetUInt64(31),
                    AttackBonus = _reader.GetInt32(32),
                    HealthBonus = _reader.GetInt32(33),
                    DefenceBonus = _reader.GetInt32(34),
                    QualityOnStart = (Quality)_reader.GetInt32(35),
                    PAS = (PreAssembledFigure)_reader.GetInt32(36),
                    Expedition = (ExpeditionCardType)_reader.GetInt32(37),
                    ExpeditionDate = _reader.GetDateTime(38),
                    GameDeckId = _reader.GetUInt64(39),
                };
            }
        }

        public override string TableName => nameof(SanakanDbContext.Cards);
    }
}
