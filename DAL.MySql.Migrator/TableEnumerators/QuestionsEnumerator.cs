using Sanakan.DAL.Models;
using System;

namespace Sanakan.DAL.MySql.Migrator.TableEnumerators
{
    public class QuestionsEnumerator : TableEnumerator<Question>
    {
        public QuestionsEnumerator(IDbConnection connection)
            : base(connection)
        {
        }

        public override Question Current => new()
        {
            Id = _reader.GetUInt64(0),
            Group = _reader.GetInt32(1),
            AnswerNumber = _reader.GetInt32(2),
            PointsWin = _reader.GetInt32(3),
            PointsLose = _reader.GetInt32(4),
            Content = _reader.GetString(5),
            TimeToAnswer = TimeSpan.FromMinutes(_reader.GetInt32(6)),
        };

        public override string TableName => nameof(SanakanDbContext.Questions);
    }
}
