﻿using DAL.Repositories.Abstractions;
using Discord.Commands;
using Discord.WebSocket;
using System;
using System.Threading.Tasks;

namespace Sanakan.Preconditions
{
    public class RequireWaifuMarketChannel : PreconditionAttribute
    {
        public async override Task<PreconditionResult> CheckPermissionsAsync(
            ICommandContext context, CommandInfo command, IServiceProvider services)
        {
            var user = context.User as SocketGuildUser;

            if (user == null)
            {
                return PreconditionResult.FromError($"To polecenie działa tylko z poziomu serwera.");
            }

            var gConfig = await db.GetCachedGuildFullConfigAsync(context.Guild.Id);

            if (gConfig == null)
            {
                return PreconditionResult.FromSuccess();
            }

            if (gConfig?.WaifuConfig?.MarketChannel != null)
            {
                if (gConfig.WaifuConfig.MarketChannel == context.Channel.Id)
                    return PreconditionResult.FromSuccess();

                if (user.GuildPermissions.Administrator)
                    return PreconditionResult.FromSuccess();

                var channel = await context.Guild.GetTextChannelAsync(gConfig.WaifuConfig.MarketChannel);
                return PreconditionResult.FromError($"To polecenie działa na kanale {channel?.Mention}");
            }

            return PreconditionResult.FromSuccess();
        }
    }
}