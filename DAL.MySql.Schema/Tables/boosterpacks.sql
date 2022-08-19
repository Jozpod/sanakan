CREATE TABLE `boosterpacks` (
  `Id` bigint unsigned NOT NULL AUTO_INCREMENT,
  `Name` varchar(50) NOT NULL,
  `TitleId` bigint unsigned DEFAULT NULL,
  `CardCount` int unsigned NOT NULL,
  `MinRarity` tinyint unsigned NOT NULL,
  `IsCardFromPackTradable` tinyint(1) NOT NULL,
  `CardSourceFromPack` tinyint unsigned NOT NULL,
  `GameDeckId` bigint unsigned NOT NULL,
  PRIMARY KEY (`Id`),
  KEY `IX_BoosterPacks_GameDeckId` (`GameDeckId`),
  CONSTRAINT `FK_BoosterPacks_GameDecks_GameDeckId` FOREIGN KEY (`GameDeckId`) REFERENCES `gamedecks` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci
ALTER TABLE boosterpacks ADD INDEX IX_BoosterPacks_GameDeckId USING BTREE(GameDeckId);
