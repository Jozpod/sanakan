CREATE TABLE `commandchannels` (
  `ChannelId` bigint unsigned NOT NULL,
  `GuildOptionsId` bigint unsigned NOT NULL,
  PRIMARY KEY (`ChannelId`,`GuildOptionsId`),
  KEY `IX_CommandChannels_GuildOptionsId` (`GuildOptionsId`),
  CONSTRAINT `FK_CommandChannels_Guilds_GuildOptionsId` FOREIGN KEY (`GuildOptionsId`) REFERENCES `guilds` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci
ALTER TABLE commandchannels ADD INDEX IX_CommandChannels_GuildOptionsId USING BTREE(GuildOptionsId);
