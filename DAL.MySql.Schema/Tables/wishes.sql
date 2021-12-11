CREATE TABLE `wishes` (
  `Id` bigint unsigned NOT NULL AUTO_INCREMENT,
  `ObjectId` bigint unsigned NOT NULL,
  `ObjectName` varchar(50) NOT NULL,
  `Type` tinyint unsigned NOT NULL,
  `GameDeckId` bigint unsigned NOT NULL,
  PRIMARY KEY (`Id`),
  UNIQUE KEY `AK_Wishes_ObjectId_GameDeckId` (`ObjectId`,`GameDeckId`),
  KEY `IX_Wishes_GameDeckId` (`GameDeckId`),
  KEY `IX_Wishes_Type_ObjectId` (`Type`,`ObjectId`),
  CONSTRAINT `FK_Wishes_GameDecks_GameDeckId` FOREIGN KEY (`GameDeckId`) REFERENCES `gamedecks` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci
ALTER TABLE wishes ADD UNIQUE INDEX AK_Wishes_ObjectId_GameDeckId USING BTREE(ObjectId, GameDeckId);
ALTER TABLE wishes ADD INDEX IX_Wishes_GameDeckId USING BTREE(GameDeckId);
ALTER TABLE wishes ADD INDEX IX_Wishes_Type_ObjectId USING BTREE(Type, ObjectId);
