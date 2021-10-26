CREATE TABLE `raports` (
  `Id` bigint unsigned NOT NULL AUTO_INCREMENT,
  `User` bigint unsigned NOT NULL,
  `Message` bigint unsigned NOT NULL,
  `GuildOptionsId` bigint unsigned NOT NULL,
  PRIMARY KEY (`Id`),
  KEY `IX_Raports_GuildOptionsId` (`GuildOptionsId`),
  CONSTRAINT `FK_Raports_Guilds_GuildOptionsId` FOREIGN KEY (`GuildOptionsId`) REFERENCES `guilds` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci
ALTER TABLE raports ADD INDEX IX_Raports_GuildOptionsId USING BTREE(GuildOptionsId);
