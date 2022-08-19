CREATE TABLE `raritysexcludedfrompacks` (
  `Rarity` tinyint unsigned NOT NULL,
  `BoosterPackId` bigint unsigned NOT NULL,
  PRIMARY KEY (`Rarity`,`BoosterPackId`),
  KEY `IX_RaritysExcludedFromPacks_BoosterPackId` (`BoosterPackId`),
  CONSTRAINT `FK_RaritysExcludedFromPacks_BoosterPacks_BoosterPackId` FOREIGN KEY (`BoosterPackId`) REFERENCES `boosterpacks` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci
ALTER TABLE raritysexcludedfrompacks ADD INDEX IX_RaritysExcludedFromPacks_BoosterPackId USING BTREE(BoosterPackId);
