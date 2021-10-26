CREATE TABLE `commandsdata` (
  `Id` bigint unsigned NOT NULL AUTO_INCREMENT,
  `UserId` bigint unsigned NOT NULL,
  `GuildId` bigint unsigned NOT NULL,
  `Date` datetime NOT NULL,
  `CommandName` varchar(50) NOT NULL,
  `CommandParameters` varchar(50) DEFAULT NULL,
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci
