CREATE TABLE `experiencecontainers` (
  `Id` bigint unsigned NOT NULL AUTO_INCREMENT,
  `ExperienceCount` double NOT NULL,
  `Level` tinyint unsigned NOT NULL,
  `GameDeckId` bigint unsigned NOT NULL,
  PRIMARY KEY (`Id`),
  UNIQUE KEY `IX_ExperienceContainers_GameDeckId` (`GameDeckId`),
  CONSTRAINT `FK_ExperienceContainers_GameDecks_GameDeckId` FOREIGN KEY (`GameDeckId`) REFERENCES `gamedecks` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci
ALTER TABLE experiencecontainers ADD UNIQUE INDEX IX_ExperienceContainers_GameDeckId USING BTREE(GameDeckId);
