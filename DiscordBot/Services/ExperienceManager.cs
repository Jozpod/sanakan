using DAL.Repositories.Abstractions;
using Discord.WebSocket;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Sanakan.DAL.Models;
using Sanakan.DAL.Models.Analytics;
using Sanakan.DiscordBot.Services;
using Sanakan.Extensions;
using Sanakan.Services.Executor;
using Sanakan.Web.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sanakan.Services
{
    public class ExperienceManager
    {
        private const double SAVE_AT = 5;
        private const double LM = 0.35;

        private Dictionary<ulong, double> _exp;
        private Dictionary<ulong, ulong> _messages;
        private Dictionary<ulong, ulong> _commands;
        private Dictionary<ulong, DateTime> _saved;
        private Dictionary<ulong, ulong> _characters;

        private DiscordSocketClient _client;
        private readonly IImageProcessing _img;
        private IExecutor _executor;
        private readonly IOptionsMonitor<SanakanConfiguration> _config;
        private readonly IRepository _repository;

        public ExperienceManager(
            DiscordSocketClient client,
            IExecutor executor,
            IOptionsMonitor<SanakanConfiguration> config,
            IImageProcessing img,
            IRepository repository)
        {
            _executor = executor;
            _client = client;
            _config = config;
            _img = img;
            _repository = repository;

            _exp = new Dictionary<ulong, double>();
            _saved = new Dictionary<ulong, DateTime>();
            _messages = new Dictionary<ulong, ulong>();
            _commands = new Dictionary<ulong, ulong>();
            _characters = new Dictionary<ulong, ulong>();

#if !DEBUG
            _client.MessageReceived += HandleMessageAsync;
#endif
        }

        public static long CalculateExpForLevel(long level, double lm = LM) => (level <= 0) ? 0 : Convert.ToInt64(Math.Floor(Math.Pow(level / lm, 2)) + 1);
        public static long CalculateLevel(long exp, double lm = LM) => Convert.ToInt64(Math.Floor(lm * Math.Sqrt(exp)));

        public async Task NotifyAboutLevelAsync(SocketGuildUser user, ISocketMessageChannel channel, long level)
        {
            using var badge = await _img.GetLevelUpBadgeAsync(
                user.Nickname ?? user.Username,
                level,
                user.GetUserOrDefaultAvatarUrl(),
                user.Roles.OrderByDescending(x => x.Position).First().Color);
            using var badgeStream = badge.ToPngStream();
            await channel.SendFileAsync(badgeStream, $"{user.Id}.png");

            var record = new UserAnalytics
            {
                Value = level,
                UserId = user.Id,
                GuildId = user.Guild.Id,
                MeasureDate = _systemClock.UtcNow,
                Type = UserAnalyticsEventType.Level
            };

            dba.UsersData.Add(record);
            _repository.SaveChanges();
        }

        private async Task HandleMessageAsync(SocketMessage message)
        {
            if (message.Author.IsBot || message.Author.IsWebhook)
            {
                return;
            }

            var user = message.Author as SocketGuildUser;

            if (user == null)
            {
                return;
            }

            if (_config.CurrentValue
                .BlacklistedGuilds.Any(x => x == user.Guild.Id))
            {
                return;
            }

            var countMsg = true;
            var calculateExp = true;

            var config = await _repository.GetCachedGuildFullConfigAsync(user.Guild.Id);
            if (config != null)
            {
                var role = user.Guild.GetRole(config.UserRole);
                if (role != null)
                {
                    if (!user.Roles.Contains(role))
                    {
                        return;
                    }
                }

                if (config.ChannelsWithoutExp != null)
                {
                    if (config.ChannelsWithoutExp.Any(x => x.Channel == message.Channel.Id))
                        calculateExp = false;
                }

                if (config.IgnoredChannels != null)
                {
                    if (config.IgnoredChannels.Any(x => x.Channel == message.Channel.Id))
                        countMsg = false;
                }
            }

            if (!_messages.Any(x => x.Key == user.Id))
            {
                if (!db.Users.AsNoTracking().Any(x => x.Id == user.Id))
                {
                    var task = CreateUserTask(user);
                    await _executor.TryAdd(new Executable("add user", task), TimeSpan.FromSeconds(1));
                }
            }

            if (countMsg)
            {
                CountMessage(user.Id, message.Content.IsCommand(_config.CurrentValue.Prefix));
            }
            CalculateExpAndCreateTask(user, message, calculateExp);
        }

        private bool CheckLastSave(ulong userId)
        {
            if (!_saved.Any(x => x.Key == userId))
            {
                _saved.Add(userId, _systemClock.UtcNow);
                return false;
            }

            return (_systemClock.UtcNow - _saved[userId].AddMinutes(30)).TotalSeconds > 1;
        }

        private void CountMessage(ulong userId, bool isCommand)
        {
            if (!_messages.Any(x => x.Key == userId))
                _messages.Add(userId, 1);
            else
                _messages[userId]++;

            if (!_commands.Any(x => x.Key == userId))
                _commands.Add(userId, isCommand ? 1u : 0u);
            else
                if (isCommand)
                _commands[userId]++;
        }

        private void CountCharacters(ulong userId, ulong characters)
        {
            if (!_characters.Any(x => x.Key == userId))
                _characters.Add(userId, characters);
            else
                _characters[userId] += characters;
        }

        private void CalculateExpAndCreateTask(SocketGuildUser user, SocketMessage message, bool calculateExp)
        {
            var exp = GetPointsFromMsg(message);
            if (!calculateExp) exp = 0;

            if (!_exp.Any(x => x.Key == user.Id))
            {
                _exp.Add(message.Author.Id, exp);
                return;
            }

            _exp[user.Id] += exp;

            var saved = _exp[user.Id];
            if (saved < SAVE_AT && !CheckLastSave(user.Id)) return;

            var fullP = (long) Math.Floor(saved);
            if (fullP < 1) return;

            _exp[message.Author.Id] -= fullP;
            _saved[user.Id] = _systemClock.UtcNow;

            var task = CreateTask(user, message.Channel, fullP, _messages[user.Id], _commands[user.Id], _characters[user.Id], calculateExp);
            _characters[user.Id] = 0;
            _messages[user.Id] = 0;
            _commands[user.Id] = 0;

            _executor.TryAdd(new Executable("add exp", task), TimeSpan.FromSeconds(1));
        }

        private double GetPointsFromMsg(SocketMessage message)
        {
            int emoteChars = message.Tags.CountEmotesTextLenght();
            int linkChars = message.Content.CountLinkTextLength();
            int nonWhiteSpaceChars = message.Content.Count(c => c != ' ');
            int quotedChars = message.Content.CountQuotedTextLength();
            double charsThatMatters = nonWhiteSpaceChars - linkChars - emoteChars - quotedChars;

            CountCharacters(message.Author.Id, (ulong)(charsThatMatters < 1 ? 1 : charsThatMatters));
            return GetExpPointBasedOnCharCount(charsThatMatters);
        }

        private double GetExpPointBasedOnCharCount(double charCount)
        {
            var tmpCnf = _config.Get();
            double cpp = tmpCnf.Exp.CharPerPoint;
            double min = tmpCnf.Exp.MinPerMessage;
            double max = tmpCnf.Exp.MaxPerMessage;

            double experience = charCount / cpp;
            if (experience < min)
            {
                return min;
            }

            if (experience > max)
            {
                return max;
            }

            return experience;
        }

        private long CheckFloodAndReturnExp(long exp, User botUser)
        {
            var fMn = botUser
                .TimeStatuses
                .FirstOrDefault(x => x.Type == StatusType.Flood);

            if (fMn == null)
            {
                fMn = StatusType.Flood.NewTimeStatus();
                botUser.TimeStatuses.Add(fMn);
            }

            if (!fMn.IsActive())
                fMn.IValue = 101;

            fMn.EndsAt = DateTime.Now.AddMinutes(10);
            if (--fMn.IValue < 20) fMn.IValue = 20;
            double ratio = fMn.IValue / 100d;

            return (long) (exp * ratio);
        }

        private Task<Task> CreateUserTask(SocketGuildUser user)
        {
            return new Task<Task>(async () =>
            {
                if (!db.Users.Any(x => x.Id == user.Id))
                {
                    var bUser = new User().Default(user.Id);
                    db.Users.Add(bUser);
                    await db.SaveChangesAsync();
                }
            });
        }

        private Task<Task> CreateTask(SocketGuildUser user, ISocketMessageChannel channel, long exp, ulong messages, ulong commands, ulong characters, bool calculateExp)
        {
            return new Task<Task>(async () =>
            {
                var usr = await _repository.GetUserOrCreateAsync(user.Id);
                if (usr == null)
                {
                    return;
                }

                if ((DateTime.Now - usr.MeasureDate.AddMonths(1)).TotalSeconds > 1)
                {
                    usr.MeasureDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
                    usr.MessagesCntAtDate = usr.MessagesCnt;
                    usr.CharacterCntFromDate = characters;
                }
                else
                    usr.CharacterCntFromDate += characters;

                exp = CheckFloodAndReturnExp(exp, usr);
                if (exp < 1) exp = 1;

                usr.ExpCnt += exp;
                usr.MessagesCnt += messages;
                usr.CommandsCnt += commands;

                var newLevel = CalculateLevel(usr.ExpCnt);
                if (newLevel != usr.Level && calculateExp)
                {
                    usr.Level = newLevel;
                    _ = Task.Run(async () => { await NotifyAboutLevelAsync(user, channel, newLevel); });
                }

                _ = Task.Run(async () =>
                {
                    var config = await _repository.GetCachedGuildFullConfigAsync(user.Guild.Id);

                    if (config == null)
                    {
                        return;
                    }

                    if (!calculateExp)
                    {
                        return;
                    }

                    foreach (var lvlRole in config.RolesPerLevel)
                    {
                        var role = user.Guild.GetRole(lvlRole.Role);
                        if (role == null)
                        {
                            continue;
                        }

                        bool hasRole = user.Roles.Any(x => x.Id == role.Id);
                        if (newLevel >= (long)lvlRole.Level)
                        {
                            if (!hasRole)
                                await user.AddRoleAsync(role);
                        }
                        else if (hasRole)
                        {
                            await user.RemoveRoleAsync(role);
                        }
                    }
                });

                await _repository.SaveChangesAsync();
            });
        }
    }
}