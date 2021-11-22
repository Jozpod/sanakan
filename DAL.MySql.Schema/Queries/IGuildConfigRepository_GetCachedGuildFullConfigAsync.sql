SELECT `t`.`Id`, `t`.`AdminRoleId`, `t`.`ChaosModeEnabled`, `t`.`GlobalEmotesRoleId`, `t`.`GoodbyeMessage`, `t`.`GreetingChannelId`, `t`.`LogChannelId`, `t`.`ModMuteRoleId`, `t`.`MuteRoleId`, `t`.`NotificationChannelId`, `t`.`NsfwChannelId`, `t`.`Prefix`, `t`.`QuizChannelId`, `t`.`RaportChannelId`, `t`.`SafariLimit`, `t`.`SupervisionEnabled`, `t`.`ToDoChannelId`, `t`.`UserRoleId`, `t`.`WaifuRoleId`, `t`.`WelcomeMessage`, `t`.`WelcomeMessagePM`, `t`.`Id0`, `t`.`DuelChannelId`, `t`.`GuildOptionsId`, `t`.`MarketChannelId`, `t`.`SpawnChannelId`, `t`.`TrashCommandsChannelId`, `t`.`TrashFightChannelId`, `t`.`TrashSpawnChannelId`
FROM (
    SELECT `g`.`Id`, `g`.`AdminRoleId`, `g`.`ChaosModeEnabled`, `g`.`GlobalEmotesRoleId`, `g`.`GoodbyeMessage`, `g`.`GreetingChannelId`, `g`.`LogChannelId`, `g`.`ModMuteRoleId`, `g`.`MuteRoleId`, `g`.`NotificationChannelId`, `g`.`NsfwChannelId`, `g`.`Prefix`, `g`.`QuizChannelId`, `g`.`RaportChannelId`, `g`.`SafariLimit`, `g`.`SupervisionEnabled`, `g`.`ToDoChannelId`, `g`.`UserRoleId`, `g`.`WaifuRoleId`, `g`.`WelcomeMessage`, `g`.`WelcomeMessagePM`, `w`.`Id` AS `Id0`, `w`.`DuelChannelId`, `w`.`GuildOptionsId`, `w`.`MarketChannelId`, `w`.`SpawnChannelId`, `w`.`TrashCommandsChannelId`, `w`.`TrashFightChannelId`, `w`.`TrashSpawnChannelId`
    FROM `Guilds` AS `g`
    LEFT JOIN `Waifus` AS `w` ON `g`.`Id` = `w`.`GuildOptionsId`
    WHERE `g`.`Id` = 1
    LIMIT 1
) AS `t`
ORDER BY `t`.`Id`, `t`.`Id0`

SELECT `i`.`Id`, `i`.`Channel`, `i`.`GuildOptionsId`, `t`.`Id`, `t`.`Id0`
FROM (
    SELECT `g`.`Id`, `w`.`Id` AS `Id0`
    FROM `Guilds` AS `g`
    LEFT JOIN `Waifus` AS `w` ON `g`.`Id` = `w`.`GuildOptionsId`
    WHERE `g`.`Id` = 1
    LIMIT 1
) AS `t`
INNER JOIN `IgnoredChannels` AS `i` ON `t`.`Id` = `i`.`GuildOptionsId`
ORDER BY `t`.`Id`, `t`.`Id0`

SELECT `w0`.`Id`, `w0`.`Channel`, `w0`.`GuildOptionsId`, `t`.`Id`, `t`.`Id0`
FROM (
    SELECT `g`.`Id`, `w`.`Id` AS `Id0`
    FROM `Guilds` AS `g`
    LEFT JOIN `Waifus` AS `w` ON `g`.`Id` = `w`.`GuildOptionsId`
    WHERE `g`.`Id` = 1
    LIMIT 1
) AS `t`
INNER JOIN `WithoutExpChannels` AS `w0` ON `t`.`Id` = `w0`.`GuildOptionsId`
ORDER BY `t`.`Id`, `t`.`Id0`

SELECT `w0`.`Id`, `w0`.`Channel`, `w0`.`GuildOptionsId`, `t`.`Id`, `t`.`Id0`
FROM (
    SELECT `g`.`Id`, `w`.`Id` AS `Id0`
    FROM `Guilds` AS `g`
    LEFT JOIN `Waifus` AS `w` ON `g`.`Id` = `w`.`GuildOptionsId`
    WHERE `g`.`Id` = 1
    LIMIT 1
) AS `t`
INNER JOIN `WithoutSupervisionChannels` AS `w0` ON `t`.`Id` = `w0`.`GuildOptionsId`
ORDER BY `t`.`Id`, `t`.`Id0`

SELECT `c`.`Id`, `c`.`ChannelId`, `c`.`GuildOptionsId`, `t`.`Id`, `t`.`Id0`
FROM (
    SELECT `g`.`Id`, `w`.`Id` AS `Id0`
    FROM `Guilds` AS `g`
    LEFT JOIN `Waifus` AS `w` ON `g`.`Id` = `w`.`GuildOptionsId`
    WHERE `g`.`Id` = 1
    LIMIT 1
) AS `t`
INNER JOIN `CommandChannels` AS `c` ON `t`.`Id` = `c`.`GuildOptionsId`
ORDER BY `t`.`Id`, `t`.`Id0`

SELECT `s`.`Id`, `s`.`GuildOptionsId`, `s`.`Name`, `s`.`Role`, `t`.`Id`, `t`.`Id0`
FROM (
    SELECT `g`.`Id`, `w`.`Id` AS `Id0`
    FROM `Guilds` AS `g`
    LEFT JOIN `Waifus` AS `w` ON `g`.`Id` = `w`.`GuildOptionsId`
    WHERE `g`.`Id` = 1
    LIMIT 1
) AS `t`
INNER JOIN `SelfRoles` AS `s` ON `t`.`Id` = `s`.`GuildOptionsId`
ORDER BY `t`.`Id`, `t`.`Id0`

SELECT `m`.`Id`, `m`.`GuildOptionsId`, `m`.`ManagerId`, `m`.`Name`, `m`.`UnderlingId`, `t`.`Id`, `t`.`Id0`
FROM (
    SELECT `g`.`Id`, `w`.`Id` AS `Id0`
    FROM `Guilds` AS `g`
    LEFT JOIN `Waifus` AS `w` ON `g`.`Id` = `w`.`GuildOptionsId`
    WHERE `g`.`Id` = 1
    LIMIT 1
) AS `t`
INNER JOIN `MyLands` AS `m` ON `t`.`Id` = `m`.`GuildOptionsId`
ORDER BY `t`.`Id`, `t`.`Id0`

SELECT `m`.`Id`, `m`.`GuildOptionsId`, `m`.`RoleId`, `t`.`Id`, `t`.`Id0`
FROM (
    SELECT `g`.`Id`, `w`.`Id` AS `Id0`
    FROM `Guilds` AS `g`
    LEFT JOIN `Waifus` AS `w` ON `g`.`Id` = `w`.`GuildOptionsId`
    WHERE `g`.`Id` = 1
    LIMIT 1
) AS `t`
INNER JOIN `ModeratorRoles` AS `m` ON `t`.`Id` = `m`.`GuildOptionsId`
ORDER BY `t`.`Id`, `t`.`Id0`

SELECT `l`.`Id`, `l`.`GuildOptionsId`, `l`.`Level`, `l`.`Role`, `t`.`Id`, `t`.`Id0`
FROM (
    SELECT `g`.`Id`, `w`.`Id` AS `Id0`
    FROM `Guilds` AS `g`
    LEFT JOIN `Waifus` AS `w` ON `g`.`Id` = `w`.`GuildOptionsId`
    WHERE `g`.`Id` = 1
    LIMIT 1
) AS `t`
INNER JOIN `LevelRoles` AS `l` ON `t`.`Id` = `l`.`GuildOptionsId`
ORDER BY `t`.`Id`, `t`.`Id0`

SELECT `w0`.`Id`, `w0`.`Channel`, `w0`.`WaifuId`, `t`.`Id`, `t`.`Id0`
FROM (
    SELECT `g`.`Id`, `w`.`Id` AS `Id0`
    FROM `Guilds` AS `g`
    LEFT JOIN `Waifus` AS `w` ON `g`.`Id` = `w`.`GuildOptionsId`
    WHERE `g`.`Id` = 1
    LIMIT 1
) AS `t`
INNER JOIN `WaifuCommandChannels` AS `w0` ON `t`.`Id0` = `w0`.`WaifuId`
ORDER BY `t`.`Id`, `t`.`Id0`

SELECT `w0`.`Id`, `w0`.`ChannelId`, `w0`.`WaifuId`, `t`.`Id`, `t`.`Id0`
FROM (
    SELECT `g`.`Id`, `w`.`Id` AS `Id0`
    FROM `Guilds` AS `g`
    LEFT JOIN `Waifus` AS `w` ON `g`.`Id` = `w`.`GuildOptionsId`
    WHERE `g`.`Id` = 1
    LIMIT 1
) AS `t`
INNER JOIN `WaifuFightChannels` AS `w0` ON `t`.`Id0` = `w0`.`WaifuId`
ORDER BY `t`.`Id`, `t`.`Id0`

SELECT `r`.`Id`, `r`.`GuildOptionsId`, `r`.`MessageId`, `r`.`UserId`, `t`.`Id`, `t`.`Id0`
FROM (
    SELECT `g`.`Id`, `w`.`Id` AS `Id0`
    FROM `Guilds` AS `g`
    LEFT JOIN `Waifus` AS `w` ON `g`.`Id` = `w`.`GuildOptionsId`
    WHERE `g`.`Id` = 1
    LIMIT 1
) AS `t`
INNER JOIN `Raports` AS `r` ON `t`.`Id` = `r`.`GuildOptionsId`
ORDER BY `t`.`Id`, `t`.`Id0`

