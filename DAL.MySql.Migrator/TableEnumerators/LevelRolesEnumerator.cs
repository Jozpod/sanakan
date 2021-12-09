using MySqlConnector;
using Sanakan.DAL.Models;
using Sanakan.DAL.Models.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sanakan.DAL.MySql.Migrator.TableEnumerators
{
    public class LevelRolesEnumerator : TableEnumerator<LevelRole>
    {
        public LevelRolesEnumerator(MySqlConnection connection)
         : base(connection) { }

        public override LevelRole Current => new LevelRole
        {
            Id = _reader.GetUInt64(0),
            RoleId = _reader.GetUInt64(1),
            Level = _reader.GetUInt64(2),
            GuildOptionsId = _reader.GetUInt64(3),
        };

        public override string TableName => "commandchannels";
    }
}
