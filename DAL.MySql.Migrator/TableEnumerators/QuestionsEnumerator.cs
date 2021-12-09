using MySqlConnector;
using Sanakan.DAL.Models;

namespace Sanakan.DAL.MySql.Migrator.TableEnumerators
{
    public class QuestionsEnumerator : TableEnumerator<Question>
    {
        public QuestionsEnumerator(MySqlConnection connection)
            : base(connection) { }

        public override Question Current => new Question
        {
            Id = _reader.GetUInt64(0),
        };

        public override string TableName => nameof(SanakanDbContext.Questions);
    }
}
