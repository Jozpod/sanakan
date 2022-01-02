using Sanakan.DAL.Models;

namespace Sanakan.DAL.MySql.Migrator.TableEnumerators
{
    public class ExperienceContainerEnumerator : TableEnumerator<ExperienceContainer>
    {
        public ExperienceContainerEnumerator(IDbConnection connection)
            : base(connection)
        {
        }

        public override ExperienceContainer Current => new ExperienceContainer
        {
            Id = _reader.GetUInt64(0),
            ExperienceCount = _reader.GetDouble(1),
            Level = (ExperienceContainerLevel)_reader.GetInt32(2),
            GameDeckId = _reader.GetUInt64(3),
        };

        public override string TableName => "expcontainers";
    }
}
