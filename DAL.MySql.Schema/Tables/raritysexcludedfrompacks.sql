CREATE TABLE `raritysexcludedfrompacks` (
  `Id` bigint unsigned NOT NULL AUTO_INCREMENT,
  `Rarity` int NOT NULL,
  `BoosterPackId` bigint unsigned NOT NULL,
  PRIMARY KEY (`Id`),
  KEY `IX_RaritysExcludedFromPacks_BoosterPackId` (`BoosterPackId`),
  CONSTRAINT `FK_RaritysExcludedFromPacks_BoosterPacks_BoosterPackId` FOREIGN KEY (`BoosterPackId`) REFERENCES `boosterpacks` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci