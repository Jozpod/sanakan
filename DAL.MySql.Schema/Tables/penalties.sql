CREATE TABLE `penalties` (
  `Id` bigint unsigned NOT NULL AUTO_INCREMENT,
  `UserId` bigint unsigned NOT NULL,
  `GuildId` bigint unsigned NOT NULL,
  `Reason` varchar(100) DEFAULT NULL,
  `Type` int NOT NULL,
  `StartDate` datetime NOT NULL,
  `Duration` time NOT NULL,
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci