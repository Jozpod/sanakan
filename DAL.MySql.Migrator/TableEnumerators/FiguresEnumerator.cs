using MySqlConnector;
using Sanakan.DAL.Models;

namespace Sanakan.DAL.MySql.Migrator.TableEnumerators
{
    public class FiguresEnumerator : TableEnumerator<Figure>
    {
        public FiguresEnumerator(MySqlConnection connection)
            : base(connection) { }

        public override Figure Current => new Figure
        {
            Id = _reader.GetUInt64(0),
        };

        public override string TableName => "Items";
    }
}
