using MySqlConnector;
using Sanakan.DAL.Models;

namespace Sanakan.DAL.MySql.Migrator.TableEnumerators
{
    public class AnswersEnumerator : TableEnumerator<Answer>
    {
        public AnswersEnumerator(MySqlConnection connection)
            : base(connection) {}

        public override Answer Current => new Answer
        {
            Id = _reader.GetUInt64(0),
            Number = _reader.GetInt32(1),
            Content = _reader.GetString(2),
            QuestionId = _reader.GetUInt64(3),
        };

        public override string TableName => nameof(SanakanDbContext.Answers);
    }
}
