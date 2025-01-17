CREATE TABLE `figures` (
  `Id` bigint unsigned NOT NULL AUTO_INCREMENT,
  `Dere` tinyint unsigned NOT NULL,
  `Attack` int NOT NULL,
  `Health` int NOT NULL,
  `Defence` int NOT NULL,
  `Name` varchar(50) NOT NULL,
  `Title` varchar(50) NOT NULL,
  `IsFocus` tinyint(1) NOT NULL,
  `ExperienceCount` double NOT NULL,
  `RestartCount` int NOT NULL,
  `Character` bigint unsigned NOT NULL,
  `IsComplete` tinyint(1) NOT NULL,
  `PAS` tinyint unsigned NOT NULL,
  `SkeletonQuality` tinyint unsigned NOT NULL,
  `CompletedOn` datetime(6) NOT NULL,
  `FocusedPart` tinyint unsigned NOT NULL,
  `PartExp` double NOT NULL,
  `HeadQuality` tinyint unsigned NOT NULL,
  `BodyQuality` tinyint unsigned NOT NULL,
  `LeftArmQuality` tinyint unsigned NOT NULL,
  `RightArmQuality` tinyint unsigned NOT NULL,
  `LeftLegQuality` tinyint unsigned NOT NULL,
  `RightLegQuality` tinyint unsigned NOT NULL,
  `ClothesQuality` tinyint unsigned NOT NULL,
  `GameDeckId` bigint unsigned NOT NULL,
  PRIMARY KEY (`Id`),
  KEY `IX_Figures_GameDeckId` (`GameDeckId`),
  CONSTRAINT `FK_Figures_GameDecks_GameDeckId` FOREIGN KEY (`GameDeckId`) REFERENCES `gamedecks` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci
ALTER TABLE figures ADD INDEX IX_Figures_GameDeckId USING BTREE(GameDeckId);
