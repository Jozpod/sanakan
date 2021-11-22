SELECT `u`.`Id`, `u`.`AcCount`, `u`.`BackgroundProfileUri`, `u`.`CharacterCountFromDate`, `u`.`CommandsCount`, `u`.`ExperienceCount`, `u`.`IsBlacklisted`, `u`.`Level`, `u`.`MeasuredOn`, `u`.`MessagesCount`, `u`.`MessagesCountAtDate`, `u`.`ProfileType`, `u`.`ScCount`, `u`.`ShindenId`, `u`.`ShowWaifuInProfile`, `u`.`StatsReplacementProfileUri`, `u`.`TcCount`, `u`.`WarningsCount`
FROM `Users` AS `u`
WHERE `u`.`Id` = 1
LIMIT 1

