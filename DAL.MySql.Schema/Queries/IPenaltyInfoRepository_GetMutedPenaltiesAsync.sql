SELECT `p`.`Id`, `p`.`Duration`, `p`.`GuildId`, `p`.`Reason`, `p`.`StartedOn`, `p`.`Type`, `p`.`UserId`, `o`.`Id`, `o`.`PenaltyInfoId`, `o`.`RoleId`
FROM `Penalties` AS `p`
LEFT JOIN `OwnedRoles` AS `o` ON `p`.`Id` = `o`.`PenaltyInfoId`
WHERE (`p`.`GuildId` = 1) AND (`p`.`Type` = 0)
ORDER BY `p`.`Id`, `o`.`Id`

