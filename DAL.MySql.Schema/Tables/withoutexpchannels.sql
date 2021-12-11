CREATE TABLE `withoutexpchannels` (
  `ChannelId` bigint unsigned NOT NULL,
  `GuildOptionsId` bigint unsigned NOT NULL,
  PRIMARY KEY (`ChannelId`,`GuildOptionsId`),
  KEY `IX_WithoutExpChannels_GuildOptionsId` (`GuildOptionsId`),
  CONSTRAINT `FK_WithoutExpChannels_Guilds_GuildOptionsId` FOREIGN KEY (`GuildOptionsId`) REFERENCES `guilds` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci
ALTER TABLE withoutexpchannels ADD INDEX IX_WithoutExpChannels_GuildOptionsId USING BTREE(GuildOptionsId);
