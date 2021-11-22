CREATE TABLE `usersdata` (
  `Id` bigint unsigned NOT NULL AUTO_INCREMENT,
  `Value` bigint unsigned NOT NULL,
  `UserId` bigint unsigned NOT NULL,
  `GuildId` bigint unsigned DEFAULT NULL,
  `MeasuredOn` datetime(6) NOT NULL,
  `Type` int NOT NULL,
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci
