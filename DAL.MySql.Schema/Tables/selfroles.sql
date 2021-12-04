CREATE TABLE `selfroles` (
  `Id` bigint unsigned NOT NULL AUTO_INCREMENT,
  `RoleId` bigint unsigned NOT NULL,
  `Name` varchar(100) NOT NULL,
  `GuildOptionsId` bigint unsigned NOT NULL,
  PRIMARY KEY (`Id`),
  KEY `IX_SelfRoles_GuildOptionsId` (`GuildOptionsId`),
  CONSTRAINT `FK_SelfRoles_Guilds_GuildOptionsId` FOREIGN KEY (`GuildOptionsId`) REFERENCES `guilds` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci
ALTER TABLE selfroles ADD INDEX IX_SelfRoles_GuildOptionsId USING BTREE(GuildOptionsId);
