SELECT `t`.`Id`, `t`.`BooleanValue`, `t`.`EndsOn`, `t`.`GuildId`, `t`.`IntegerValue`, `t`.`Type`, `t`.`UserId`
FROM `TimeStatuses` AS `t`
WHERE `t`.`Type` IN (3, 2)

