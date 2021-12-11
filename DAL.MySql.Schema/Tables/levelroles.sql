CREATE TABLE `levelroles` (
  `RoleId` bigint unsigned NOT NULL,
  `GuildOptionsId` bigint unsigned NOT NULL,
  `Level` bigint unsigned NOT NULL,
  PRIMARY KEY (`RoleId`,`GuildOptionsId`),
  KEY `IX_LevelRoles_GuildOptionsId` (`GuildOptionsId`),
  CONSTRAINT `FK_LevelRoles_Guilds_GuildOptionsId` FOREIGN KEY (`GuildOptionsId`) REFERENCES `guilds` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci
ALTER TABLE levelroles ADD INDEX IX_LevelRoles_GuildOptionsId USING BTREE(GuildOptionsId);
