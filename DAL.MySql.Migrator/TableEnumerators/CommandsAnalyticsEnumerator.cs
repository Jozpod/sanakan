using MySqlConnector;
using Sanakan.DAL.Models;
using Sanakan.DAL.Models.Analytics;
using Sanakan.DAL.Models.Configuration;
using Sanakan.DAL.MySql.Migrator.TableEnumerators;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sanakan.DAL.MySql.Migrator.TableEnumerators
{
    public class CommandsAnalyticsEnumerator : TableEnumerator<CommandsAnalytics>
    {
        public CommandsAnalyticsEnumerator(MySqlConnection connection)
          : base(connection) { }

        public override CommandsAnalytics Current => new CommandsAnalytics
        {
            Id = _reader.GetUInt64(0),
        };

        public override string TableName => nameof(SanakanDbContext.CommandsData);
    }
}
