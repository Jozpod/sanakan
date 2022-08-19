SELECT `p`.`Id`, `p`.`Duration`, `p`.`GuildId`, `p`.`Reason`, `p`.`StartedOn`, `p`.`Type`, `p`.`UserId`
FROM `Penalties` AS `p`
WHERE `p`.`GuildId` = 1

