CREATE TABLE `transferdata` (
  `Id` bigint unsigned NOT NULL AUTO_INCREMENT,
  `Value` bigint unsigned NOT NULL,
  `CreatedOn` datetime(6) NOT NULL,
  `DiscordUserId` bigint unsigned NOT NULL,
  `ShindenUserId` bigint unsigned NOT NULL,
  `Source` tinyint unsigned NOT NULL,
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci
