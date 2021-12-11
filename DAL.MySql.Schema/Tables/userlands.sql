CREATE TABLE `userlands` (
  `Id` bigint unsigned NOT NULL AUTO_INCREMENT,
  `Name` varchar(100) NOT NULL,
  `ManagerId` bigint unsigned NOT NULL,
  `UnderlingId` bigint unsigned NOT NULL,
  `GuildOptionsId` bigint unsigned NOT NULL,
  PRIMARY KEY (`Id`),
  KEY `IX_UserLands_GuildOptionsId` (`GuildOptionsId`),
  CONSTRAINT `FK_UserLands_Guilds_GuildOptionsId` FOREIGN KEY (`GuildOptionsId`) REFERENCES `guilds` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci
ALTER TABLE userlands ADD INDEX IX_UserLands_GuildOptionsId USING BTREE(GuildOptionsId);
