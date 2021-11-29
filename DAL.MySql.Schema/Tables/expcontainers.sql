CREATE TABLE `expcontainers` (
  `Id` bigint unsigned NOT NULL AUTO_INCREMENT,
  `ExperienceCount` double NOT NULL,
  `Level` int NOT NULL,
  `GameDeckId` bigint unsigned NOT NULL,
  PRIMARY KEY (`Id`),
  UNIQUE KEY `IX_ExpContainers_GameDeckId` (`GameDeckId`),
  CONSTRAINT `FK_ExpContainers_GameDecks_GameDeckId` FOREIGN KEY (`GameDeckId`) REFERENCES `gamedecks` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB AUTO_INCREMENT=4 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci
ALTER TABLE expcontainers ADD UNIQUE INDEX IX_ExpContainers_GameDeckId USING BTREE(GameDeckId);
