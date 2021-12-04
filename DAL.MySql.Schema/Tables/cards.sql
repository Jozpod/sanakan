CREATE TABLE `cards` (
  `Id` bigint unsigned NOT NULL AUTO_INCREMENT,
  `Active` tinyint(1) NOT NULL,
  `InCage` tinyint(1) NOT NULL,
  `IsTradable` tinyint(1) NOT NULL,
  `ExperienceCount` double NOT NULL,
  `Affection` double NOT NULL,
  `UpgradesCount` int NOT NULL,
  `RestartCount` int NOT NULL,
  `Rarity` int NOT NULL,
  `RarityOnStart` int NOT NULL,
  `Dere` int NOT NULL,
  `Defence` int NOT NULL,
  `Attack` int NOT NULL,
  `Health` int NOT NULL,
  `Name` varchar(50) NOT NULL,
  `CharacterId` bigint unsigned NOT NULL,
  `CreatedOn` datetime(4) NOT NULL,
  `Source` int NOT NULL,
  `Title` varchar(50) DEFAULT NULL,
  `ImageUrl` varchar(50) DEFAULT NULL,
  `CustomImageUrl` varchar(50) DEFAULT NULL,
  `FirstOwnerId` bigint unsigned DEFAULT NULL,
  `LastOwnerId` bigint unsigned DEFAULT NULL,
  `IsUnique` tinyint(1) NOT NULL,
  `StarStyle` int NOT NULL,
  `CustomBorderUrl` varchar(50) DEFAULT NULL,
  `MarketValue` double NOT NULL,
  `Curse` int NOT NULL,
  `CardPower` double NOT NULL,
  `EnhanceCount` int NOT NULL,
  `FromFigure` tinyint(1) NOT NULL,
  `Quality` int NOT NULL,
  `AttackBonus` int NOT NULL,
  `HealthBonus` int NOT NULL,
  `DefenceBonus` int NOT NULL,
  `QualityOnStart` int NOT NULL,
  `PAS` int NOT NULL,
  `Expedition` int NOT NULL,
  `ExpeditionDate` datetime(4) NOT NULL,
  `GameDeckId` bigint unsigned NOT NULL,
  PRIMARY KEY (`Id`),
  KEY `IX_Cards_Active` (`Active`),
  KEY `IX_Cards_CharacterId` (`CharacterId`),
  KEY `IX_Cards_GameDeckId` (`GameDeckId`),
  KEY `IX_Cards_Title` (`Title`),
  CONSTRAINT `FK_Cards_GameDecks_GameDeckId` FOREIGN KEY (`GameDeckId`) REFERENCES `gamedecks` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `CK_Card_CustomBorderUrl` CHECK ((regexp_like(`CustomBorderUrl`,_utf8mb4'^https?') or (`CustomBorderUrl` is null))),
  CONSTRAINT `CK_Card_CustomImageUrl` CHECK ((regexp_like(`CustomImageUrl`,_utf8mb4'^https?') or (`CustomImageUrl` is null))),
  CONSTRAINT `CK_Card_ImageUrl` CHECK ((regexp_like(`ImageUrl`,_utf8mb4'^https?') or (`ImageUrl` is null))),
  CONSTRAINT `CK_Card_Title` CHECK (((trim(`Title`) <> _utf8mb4'') or (`Title` is null)))
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci
ALTER TABLE cards ADD INDEX IX_Cards_Active USING BTREE(Active);
ALTER TABLE cards ADD INDEX IX_Cards_CharacterId USING BTREE(CharacterId);
ALTER TABLE cards ADD INDEX IX_Cards_GameDeckId USING BTREE(GameDeckId);
ALTER TABLE cards ADD INDEX IX_Cards_Title USING BTREE(Title);
