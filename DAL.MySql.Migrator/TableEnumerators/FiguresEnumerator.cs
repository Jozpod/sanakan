using Sanakan.DAL.Models;

namespace Sanakan.DAL.MySql.Migrator.TableEnumerators
{
    public class FiguresEnumerator : TableEnumerator<Figure>
    {
        public FiguresEnumerator(IDbConnection connection)
            : base(connection) { }

        public override Figure Current => new()
        {
            Id = _reader.GetUInt64(0),
            Dere = (Dere)_reader.GetInt32(1),
            Attack = _reader.GetInt32(2),
            Health = _reader.GetInt32(3),
            Defence = _reader.GetInt32(4),
            Name = _reader.GetString(5),
            Title = _reader.GetString(6),
            IsFocus = _reader.GetBoolean(7),
            ExperienceCount = _reader.GetUInt64(8),
            RestartCount = _reader.GetInt32(9),
            Character = _reader.GetUInt64(10),
            IsComplete = _reader.GetBoolean(11),
            PAS = (PreAssembledFigure)_reader.GetUInt64(12),
            SkeletonQuality = (Quality)_reader.GetUInt64(13),
            CompletedOn = _reader.GetDateTime(14),
            FocusedPart = (FigurePart)_reader.GetUInt64(15),
            PartExp = _reader.GetDouble(16),
            HeadQuality = (Quality)_reader.GetInt32(17),
            BodyQuality = (Quality)_reader.GetInt32(18),
            LeftArmQuality = (Quality)_reader.GetInt32(19),
            RightArmQuality = (Quality)_reader.GetInt32(20),
            LeftLegQuality = (Quality)_reader.GetInt32(21),
            RightLegQuality = (Quality)_reader.GetInt32(22),
            ClothesQuality = (Quality)_reader.GetInt32(23),
            GameDeckId = _reader.GetUInt64(24),
        };

        public override string TableName => nameof(SanakanDbContext.Figures);
    }
}
