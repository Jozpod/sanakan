using Microsoft.Extensions.Options;
using Sanakan.Common.Configuration;
using Sanakan.DAL;
using Sanakan.DAL.Models;
using Sanakan.DAL.Models.Configuration;
using Sanakan.Game;
using Sanakan.Game.Services.Abstractions;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;

namespace Sanakan.Web
{
    [ExcludeFromCodeCoverage]
    public class DatabaseSeeder
    {
        private readonly SanakanDbContext _dbContext;
        private readonly IDatabaseFacade _databaseFacade;
        private readonly IWaifuService _waifuService;
        private readonly DatabaseSeedConfiguration _configuration;

        public DatabaseSeeder(
            SanakanDbContext dbContext,
            IDatabaseFacade databaseFacade,
            IWaifuService waifuService,
            IOptions<DatabaseSeedConfiguration> configuration)
        {
            _dbContext = dbContext;
            _databaseFacade = databaseFacade;
            _waifuService = waifuService;
            _configuration = configuration.Value;
        }

        public async Task RunAsync()
        {
            if (_configuration.Enabled)
            {
                return;
            }

            if(!await _databaseFacade.EnsureCreatedAsync())
            {
                return;
            }

            foreach (var guild in _configuration.Guilds)
            {
                var guildOption = new GuildOptions(guild.Id, guild.SafariLimit);

                _dbContext.Guilds.Add(guildOption);
                await _dbContext.SaveChangesAsync();

                foreach (var userSeed in guild.Users)
                {
                    var user = new User(userSeed.Id, DateTime.UtcNow);
                    _dbContext.Users.Add(user);
                    await _dbContext.SaveChangesAsync();

                    if (userSeed.ScCount.HasValue)
                    {
                        user.ScCount = (long)userSeed.ScCount.Value;
                    }

                    if (userSeed.AcCount.HasValue)
                    {
                        user.AcCount = (long)userSeed.AcCount.Value;
                    }

                    if (userSeed.TcCount.HasValue)
                    {
                        user.TcCount = (long)userSeed.TcCount.Value;
                    }

                    if (userSeed.CommandsCount.HasValue)
                    {
                        user.CommandsCount = userSeed.CommandsCount.Value;
                    }

                    if (userSeed.NumberOfCards.HasValue)
                    {
                        foreach (var _ in Enumerable.Range(1, userSeed.NumberOfCards.Value))
                        {
                            var character = await _waifuService.GetRandomCharacterAsync();
                            var card = _waifuService.GenerateNewCard(user.Id, character!);
                            user.GameDeck.Cards.Add(card);
                        }
                        await _dbContext.SaveChangesAsync();
                    }

                    if (userSeed.NumberOfItems.HasValue)
                    {
                        foreach (var item in Enum.GetValues<ItemType>())
                        {
                            user.GameDeck.Items.Add(item.ToItem(10, Quality.Alpha));
                        }
                        await _dbContext.SaveChangesAsync();
                    }

                    if (userSeed.ProfileType.HasValue)
                    {
                        user.ProfileType = userSeed.ProfileType.Value;
                    }

                    if (userSeed.PVPCoins.HasValue)
                    {
                        user.GameDeck.PVPCoins = (long)userSeed.PVPCoins.Value;
                    }

                    if (userSeed.ExperiencePercentage.HasValue)
                    {
                        user.ExperienceCount = (ulong)userSeed.ExperiencePercentage.Value * ExperienceUtils.CalculateExpForLevel(user.Level + 1);
                    }

                    await _dbContext.SaveChangesAsync();
                }
            }
        }
    }
}
