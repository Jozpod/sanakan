CREATE TABLE `transferdata` (
  `Id` bigint unsigned NOT NULL AUTO_INCREMENT,
  `Value` bigint NOT NULL,
  `Date` datetime NOT NULL,
  `DiscordId` bigint unsigned NOT NULL,
  `ShindenId` bigint unsigned NOT NULL,
  `Source` int NOT NULL,
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci