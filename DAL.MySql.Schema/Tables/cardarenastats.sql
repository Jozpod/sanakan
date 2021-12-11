CREATE TABLE `cardarenastats` (
  `Id` bigint unsigned NOT NULL AUTO_INCREMENT,
  `WinsCount` bigint NOT NULL,
  `LosesCount` bigint NOT NULL,
  `DrawsCount` bigint NOT NULL,
  `CardId` bigint unsigned NOT NULL,
  PRIMARY KEY (`Id`),
  UNIQUE KEY `IX_CardArenaStats_CardId` (`CardId`),
  CONSTRAINT `FK_CardArenaStats_Cards_CardId` FOREIGN KEY (`CardId`) REFERENCES `cards` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci
ALTER TABLE cardarenastats ADD UNIQUE INDEX IX_CardArenaStats_CardId USING BTREE(CardId);
