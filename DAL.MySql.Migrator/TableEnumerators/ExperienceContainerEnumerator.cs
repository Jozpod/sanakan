using MySqlConnector;
using Sanakan.DAL.Models;
using Sanakan.DAL.Models.Analytics;

namespace Sanakan.DAL.MySql.Migrator.TableEnumerators
{
    public class ExperienceContainerEnumerator : TableEnumerator<ExperienceContainer>
    {
        public ExperienceContainerEnumerator(MySqlConnection connection)
            : base(connection) { }

        public override ExperienceContainer Current => new ExperienceContainer
        {
            Id = _reader.GetUInt64(0),
        };

        public override string TableName => "ExpContainers";
    }
}
