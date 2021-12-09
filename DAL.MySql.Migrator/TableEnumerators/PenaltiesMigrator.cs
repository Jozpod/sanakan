using MySqlConnector;
using Sanakan.DAL.Models;
using Sanakan.DAL.Models.Management;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sanakan.DAL.MySql.Migrator.TableEnumerators
{
    public class PenaltiesEnumerator : TableEnumerator<PenaltyInfo>
    {
        public PenaltiesEnumerator(MySqlConnection connection)
            : base(connection) { }

        public override PenaltyInfo Current => new PenaltyInfo
        {
            Id = _reader.GetUInt64(0),
            UserId = _reader.GetUInt64(1),
            GuildId = _reader.GetUInt64(2),
            Reason = _reader.GetString(3),
            Type = (PenaltyType)_reader.GetInt32(4),
            StartedOn = _reader.GetDateTime(5),
            Duration = TimeSpan.FromHours(_reader.GetInt64(7)),
        };

        public override string TableName => "penalties";
    }
}
