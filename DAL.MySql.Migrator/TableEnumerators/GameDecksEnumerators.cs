using MySqlConnector;
using Sanakan.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sanakan.DAL.MySql.Migrator.TableEnumerators
{
    public class GameDecksEnumerators : TableEnumerator<GameDeck>
    {
        public GameDecksEnumerators(MySqlConnection connection)
           : base(connection) { }

        public override GameDeck Current => new GameDeck
        {
            Id = _reader.GetUInt64(0),
        };

        public override string TableName => "Items";
    }
}
