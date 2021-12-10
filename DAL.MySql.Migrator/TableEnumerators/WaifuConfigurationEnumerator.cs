using Sanakan.DAL.Models.Configuration;

namespace Sanakan.DAL.MySql.Migrator.TableEnumerators
{
    public class WaifuConfigurationEnumerator : TableEnumerator<WaifuConfiguration>
    {
        public WaifuConfigurationEnumerator(IDbConnection connection)
            : base(connection) { }

        public override WaifuConfiguration Current
        {
            get
            {
                ulong? marketChannelId = _reader.GetUInt64(1);
                marketChannelId = marketChannelId == 0 ? null : marketChannelId;
                ulong? spawnChannelId = _reader.GetUInt64(2);
                spawnChannelId = spawnChannelId == 0 ? null : spawnChannelId;
                ulong? duelChannelId = _reader.GetUInt64(3);
                duelChannelId = duelChannelId == 0 ? null : duelChannelId;
                ulong? trashFightChannelId = _reader.GetUInt64(4);
                trashFightChannelId = trashFightChannelId == 0 ? null : trashFightChannelId;
                ulong? trashSpawnChannelId = _reader.GetUInt64(5);
                trashSpawnChannelId = trashSpawnChannelId == 0 ? null : trashSpawnChannelId;
                ulong? trashCommandsChannelId = _reader.GetUInt64(6);
                trashCommandsChannelId = trashCommandsChannelId == 0 ? null : trashCommandsChannelId;

                return new()
                {
                    Id = _reader.GetUInt64(0),
                    MarketChannelId = marketChannelId,
                    SpawnChannelId = spawnChannelId,
                    DuelChannelId = duelChannelId,
                    TrashFightChannelId = trashFightChannelId,
                    TrashSpawnChannelId = trashSpawnChannelId,
                    TrashCommandsChannelId = trashCommandsChannelId,
                    GuildOptionsId = _reader.GetUInt64(7),
                };
            }
        }

        public override string TableName => nameof(SanakanDbContext.Waifus);
    }
}
