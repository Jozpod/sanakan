CREATE TABLE `moderatorroles` (
  `RoleId` bigint unsigned NOT NULL,
  `GuildOptionsId` bigint unsigned NOT NULL,
  PRIMARY KEY (`RoleId`,`GuildOptionsId`),
  KEY `IX_ModeratorRoles_GuildOptionsId` (`GuildOptionsId`),
  CONSTRAINT `FK_ModeratorRoles_Guilds_GuildOptionsId` FOREIGN KEY (`GuildOptionsId`) REFERENCES `guilds` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci
ALTER TABLE moderatorroles ADD INDEX IX_ModeratorRoles_GuildOptionsId USING BTREE(GuildOptionsId);
