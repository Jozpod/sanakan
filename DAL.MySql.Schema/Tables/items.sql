CREATE TABLE `items` (
  `Id` bigint unsigned NOT NULL AUTO_INCREMENT,
  `Count` bigint NOT NULL,
  `Name` longtext NOT NULL,
  `Type` int NOT NULL,
  `Quality` int NOT NULL,
  `GameDeckId` bigint unsigned NOT NULL,
  PRIMARY KEY (`Id`),
  KEY `IX_Items_GameDeckId` (`GameDeckId`),
  CONSTRAINT `FK_Items_GameDecks_GameDeckId` FOREIGN KEY (`GameDeckId`) REFERENCES `gamedecks` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci
ALTER TABLE items ADD INDEX IX_Items_GameDeckId USING BTREE(GameDeckId);
