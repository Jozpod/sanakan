using Microsoft.Extensions.Options;
using Sanakan.Common;
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
    public class DatabaseSeeder : IDatabaseSeeder
    {
        private readonly SanakanDbContext _dbContext;
        private readonly IDatabaseFacade _databaseFacade;
        private readonly IWaifuService _waifuService;
        private readonly IRandomNumberGenerator _randomNumberGenerator;
        private readonly DatabaseSeedConfiguration _configuration;

        public DatabaseSeeder(
            SanakanDbContext dbContext,
            IDatabaseFacade databaseFacade,
            IWaifuService waifuService,
            IRandomNumberGenerator randomNumberGenerator,
            IOptions<DatabaseSeedConfiguration> configuration)
        {
            _dbContext = dbContext;
            _databaseFacade = databaseFacade;
            _waifuService = waifuService;
            _randomNumberGenerator = randomNumberGenerator;
            _configuration = configuration.Value;
        }

        [SuppressMessage("Microsoft.Analyzers.ManagedCodeAnalysis", "CA1502:AvoidExcessiveComplexity", Justification = "Resolved")]
        public async Task RunAsync()
        {
            if (!_configuration.Enabled)
            {
                return;
            }

            if (!await _databaseFacade.EnsureCreatedAsync())
            {
                return;
            }

            foreach (var guild in _configuration.Guilds)
            {
                var guildOption = new GuildOptions(guild.Id, guild.SafariLimit ?? 50);

                if (guild.ModMuteRoleId.HasValue)
                {
                    guildOption.ModMuteRoleId = guild.ModMuteRoleId.Value;
                }

                if (guild.MuteRoleId.HasValue)
                {
                    guildOption.MuteRoleId = guild.MuteRoleId.Value;
                }

                if (guild.UserRoleId.HasValue)
                {
                    guildOption.UserRoleId = guild.UserRoleId.Value;
                }

                if (guild.WaifuRoleId.HasValue)
                {
                    guildOption.WaifuRoleId = guild.WaifuRoleId.Value;
                }

                if (guild.AdminRoleId.HasValue)
                {
                    guildOption.AdminRoleId = guild.AdminRoleId.Value;
                }

                if (guild.LogChannelId.HasValue)
                {
                    guildOption.LogChannelId = guild.LogChannelId.Value;
                }

                if (guild.GlobalEmotesRoleId.HasValue)
                {
                    guildOption.GlobalEmotesRoleId = guild.GlobalEmotesRoleId.Value;
                }

                if (guild.GreetingChannelId.HasValue)
                {
                    guildOption.GreetingChannelId = guild.GreetingChannelId.Value;
                }

                if (guild.WelcomeMessage != null)
                {
                    guildOption.WelcomeMessage = guild.WelcomeMessage;
                }

                if (guild.CommandChannelId.HasValue)
                {
                    guildOption.CommandChannels.Add(new CommandChannel { ChannelId = guild.CommandChannelId.Value });
                }

                if (guild.CommandWaifuChannelId.HasValue)
                {
                    guildOption.WaifuConfig ??= new WaifuConfiguration();
                    guildOption.WaifuConfig.CommandChannels.Add(new WaifuCommandChannel { ChannelId = guild.CommandWaifuChannelId.Value });
                }

                if (guild.NotificationChannelId.HasValue)
                {
                    guildOption.NotificationChannelId = guild.NotificationChannelId.Value;
                }

                if (guild.FightWaifuChannelId.HasValue)
                {
                    guildOption.WaifuConfig ??= new WaifuConfiguration();
                    guildOption.WaifuConfig.FightChannels.Add(new WaifuFightChannel { ChannelId = guild.FightWaifuChannelId.Value });
                }

                if (guild.SpawnChannelId.HasValue)
                {
                    guildOption.WaifuConfig ??= new WaifuConfiguration();
                    guildOption.WaifuConfig.SpawnChannelId = guild.SpawnChannelId.Value;
                }

                if (guild.TrashSpawnChannelId.HasValue)
                {
                    guildOption.WaifuConfig ??= new WaifuConfiguration();
                    guildOption.WaifuConfig.TrashSpawnChannelId = guild.TrashSpawnChannelId.Value;
                }

                if (guild.TodoChannelId.HasValue)
                {
                    guildOption.ToDoChannelId = guild.TodoChannelId.Value;
                }

                if (guild.ReportChannelId.HasValue)
                {
                    guildOption.RaportChannelId = guild.ReportChannelId.Value;
                }

                if (guild.IgnoredChannelId.HasValue)
                {
                    guildOption.IgnoredChannels.Add(new WithoutMessageCountChannel { ChannelId = guild.IgnoredChannelId.Value });
                }

                if (guild.TrashCommandsChannelId.HasValue)
                {
                    guildOption.WaifuConfig ??= new WaifuConfiguration();
                    guildOption.WaifuConfig.TrashCommandsChannelId = guild.TrashCommandsChannelId.Value;
                }

                if (guild.NsfwChannelId.HasValue)
                {
                    guildOption.NsfwChannelId = guild.NsfwChannelId.Value;
                }

                if (guild.NonExpChannelId.HasValue)
                {
                    guildOption.ChannelsWithoutExperience.Add(new WithoutExpChannel { ChannelId = guild.NonExpChannelId.Value });
                }

                if (guild.NonSupChannelId.HasValue)
                {
                    guildOption.ChannelsWithoutSupervision.Add(new WithoutSupervisionChannel { ChannelId = guild.NonSupChannelId.Value });
                }

                if (guild.SafariWaifuChannelId.HasValue)
                {
                    guildOption.WaifuConfig ??= new WaifuConfiguration();
                    guildOption.WaifuConfig.SpawnChannelId = guild.SafariWaifuChannelId.Value;
                }

                foreach (var rolesPerLevel in guild.RolesPerLevel)
                {
                    guildOption.RolesPerLevel.Add(new LevelRole { Level = rolesPerLevel.Level, RoleId = rolesPerLevel.RoleId });
                }

                _dbContext.Guilds.Add(guildOption);
                await _dbContext.SaveChangesAsync();

                foreach (var userSeed in guild.Users)
                {
                    var user = new User(userSeed.Id, DateTime.UtcNow);
                    _dbContext.Users.Add(user);

                    if (userSeed.Karma.HasValue)
                    {
                        user.GameDeck.Karma = userSeed.Karma.Value;
                    }

                    if (userSeed.ShindenId.HasValue)
                    {
                        user.ShindenId = userSeed.ShindenId.Value;
                    }

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

                    if (userSeed.Karma.HasValue)
                    {
                        user.GameDeck.Karma = userSeed.Karma.Value;
                    }

                    if (userSeed.CommandsCount.HasValue)
                    {
                        user.CommandsCount = userSeed.CommandsCount.Value;
                    }

                    await _dbContext.SaveChangesAsync();

                    if (userSeed.NumberOfCards.HasValue)
                    {
                        var couldNotFindCharacters = false;

                        foreach (var it in Enumerable.Range(1, userSeed.NumberOfCards.Value))
                        {
                            var character = await _waifuService.GetRandomCharacterAsync();

                            if(character == null)
                            {
                                couldNotFindCharacters = true;
                                break;
                            }

                            var card = _waifuService.GenerateNewCard(user.Id, character!);
                            card.Active = true;

                            if (userSeed.DefaultAffection.HasValue)
                            {
                                card.Affection = userSeed.DefaultAffection.Value;
                            }

                            user.GameDeck.Cards.Add(card);
                        }

                        var activeCards = new List<Card>();
                        var cards = user.GameDeck.Cards.ToList();
                        var cardsToActivate = userSeed.ActiveCards.HasValue && !couldNotFindCharacters ?
                            Enumerable.Range(1, userSeed.ActiveCards.Value)
                            : Enumerable.Empty<int>();
                        var wishListItems = userSeed.WishListItems.HasValue && !couldNotFindCharacters ?
                            Enumerable.Range(1, userSeed.WishListItems.Value)
                            : Enumerable.Empty<int>();

                        foreach (var it in cardsToActivate)
                        {
                            var card = _randomNumberGenerator.GetOneRandomFrom(cards);
                            card.Active = true;
                            activeCards.Add(card);
                            cards.Remove(card);
                        }

                        user.GameDeck.DeckPower = activeCards.Sum(x => x.CalculateCardPower());

                        foreach (var it in wishListItems)
                        {
                            var character = await _waifuService.GetRandomCharacterAsync();

                            user.GameDeck.Wishes.Add(new WishlistObject
                            {
                                ObjectId = character.CharacterId,
                                Type = WishlistObjectType.Character,
                                ObjectName = character.ToString(),
                            });
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
