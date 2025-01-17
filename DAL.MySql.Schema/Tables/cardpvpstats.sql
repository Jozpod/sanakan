CREATE TABLE `cardpvpstats` (
  `Id` bigint unsigned NOT NULL AUTO_INCREMENT,
  `Type` tinyint unsigned NOT NULL,
  `Result` tinyint unsigned NOT NULL,
  `GameDeckId` bigint unsigned NOT NULL,
  PRIMARY KEY (`Id`),
  KEY `IX_CardPvPStats_GameDeckId` (`GameDeckId`),
  CONSTRAINT `FK_CardPvPStats_GameDecks_GameDeckId` FOREIGN KEY (`GameDeckId`) REFERENCES `gamedecks` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci
ALTER TABLE cardpvpstats ADD INDEX IX_CardPvPStats_GameDeckId USING BTREE(GameDeckId);
