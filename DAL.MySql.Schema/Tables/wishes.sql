CREATE TABLE `wishes` (
  `Id` bigint unsigned NOT NULL AUTO_INCREMENT,
  `ObjectId` bigint unsigned NOT NULL,
  `ObjectName` varchar(100) NOT NULL,
  `Type` int NOT NULL,
  `GameDeckId` bigint unsigned NOT NULL,
  PRIMARY KEY (`Id`),
  KEY `IX_Wishes_GameDeckId` (`GameDeckId`),
  CONSTRAINT `FK_Wishes_GameDecks_GameDeckId` FOREIGN KEY (`GameDeckId`) REFERENCES `gamedecks` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci