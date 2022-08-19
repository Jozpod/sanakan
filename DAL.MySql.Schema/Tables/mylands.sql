CREATE TABLE `mylands` (
  `Id` bigint unsigned NOT NULL AUTO_INCREMENT,
  `Name` varchar(100) NOT NULL,
  `ManagerId` bigint unsigned NOT NULL,
  `UnderlingId` bigint unsigned NOT NULL,
  `GuildOptionsId` bigint unsigned NOT NULL,
  PRIMARY KEY (`Id`),
  KEY `IX_MyLands_GuildOptionsId` (`GuildOptionsId`),
  CONSTRAINT `FK_MyLands_Guilds_GuildOptionsId` FOREIGN KEY (`GuildOptionsId`) REFERENCES `guilds` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci
ALTER TABLE mylands ADD INDEX IX_MyLands_GuildOptionsId USING BTREE(GuildOptionsId);
