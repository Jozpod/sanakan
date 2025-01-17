CREATE TABLE `waifucommandchannels` (
  `ChannelId` bigint unsigned NOT NULL,
  `WaifuId` bigint unsigned NOT NULL,
  PRIMARY KEY (`ChannelId`,`WaifuId`),
  KEY `IX_WaifuCommandChannels_WaifuId` (`WaifuId`),
  CONSTRAINT `FK_WaifuCommandChannels_Waifus_WaifuId` FOREIGN KEY (`WaifuId`) REFERENCES `waifus` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci
ALTER TABLE waifucommandchannels ADD INDEX IX_WaifuCommandChannels_WaifuId USING BTREE(WaifuId);
