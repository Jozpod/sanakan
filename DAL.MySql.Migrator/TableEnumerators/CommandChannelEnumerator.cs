using MySqlConnector;
using Sanakan.DAL.Models;
using Sanakan.DAL.Models.Configuration;
using Sanakan.DAL.MySql.Migrator.TableEnumerators;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sanakan.DAL.MySql.Migrator.TableEnumerators
{
    public class CommandChannelEnumerator : TableEnumerator<CommandChannel>
    {
        public CommandChannelEnumerator(MySqlConnection connection)
          : base(connection) { }

        public override CommandChannel Current => new CommandChannel
        {
            Id = _reader.GetUInt64(0),
            ChannelId = _reader.GetUInt64(1),
            GuildOptionsId = _reader.GetUInt64(2),
        };

        public override string TableName => nameof(SanakanDbContext.CommandChannels);
    }
}
