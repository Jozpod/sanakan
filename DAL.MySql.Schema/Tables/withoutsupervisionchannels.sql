CREATE TABLE `withoutsupervisionchannels` (
  `Id` bigint unsigned NOT NULL AUTO_INCREMENT,
  `Channel` bigint unsigned NOT NULL,
  `GuildOptionsId` bigint unsigned NOT NULL,
  PRIMARY KEY (`Id`),
  KEY `IX_WithoutSupervisionChannels_GuildOptionsId` (`GuildOptionsId`),
  CONSTRAINT `FK_WithoutSupervisionChannels_Guilds_GuildOptionsId` FOREIGN KEY (`GuildOptionsId`) REFERENCES `guilds` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci
ALTER TABLE withoutsupervisionchannels ADD INDEX IX_WithoutSupervisionChannels_GuildOptionsId USING BTREE(GuildOptionsId);