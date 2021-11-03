CREATE TABLE `boosterpackcharacters` (
  `Id` bigint unsigned NOT NULL AUTO_INCREMENT,
  `CharacterId` bigint unsigned NOT NULL,
  `BoosterPackId` bigint unsigned NOT NULL,
  PRIMARY KEY (`Id`),
  KEY `IX_BoosterPackCharacters_BoosterPackId` (`BoosterPackId`),
  CONSTRAINT `FK_BoosterPackCharacters_BoosterPacks_BoosterPackId` FOREIGN KEY (`BoosterPackId`) REFERENCES `boosterpacks` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci
ALTER TABLE boosterpackcharacters ADD INDEX IX_BoosterPackCharacters_BoosterPackId USING BTREE(BoosterPackId);
