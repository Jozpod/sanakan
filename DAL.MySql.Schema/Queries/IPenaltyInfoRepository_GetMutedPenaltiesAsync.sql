SELECT `p`.`Id`, `p`.`Duration`, `p`.`GuildId`, `p`.`Reason`, `p`.`StartedOn`, `p`.`Type`, `p`.`UserId`, `o`.`RoleId`, `o`.`PenaltyInfoId`
FROM `Penalties` AS `p`
LEFT JOIN `OwnedRoles` AS `o` ON `p`.`Id` = `o`.`PenaltyInfoId`
WHERE (`p`.`GuildId` = 1) AND (`p`.`Type` = 0)
ORDER BY `p`.`Id`, `o`.`RoleId`, `o`.`PenaltyInfoId`

