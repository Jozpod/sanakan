CREATE TABLE `waifufightchannels` (
  `Id` bigint unsigned NOT NULL AUTO_INCREMENT,
  `Channel` bigint unsigned NOT NULL,
  `WaifuId` bigint unsigned NOT NULL,
  PRIMARY KEY (`Id`),
  KEY `IX_WaifuFightChannels_WaifuId` (`WaifuId`),
  CONSTRAINT `FK_WaifuFightChannels_Waifus_WaifuId` FOREIGN KEY (`WaifuId`) REFERENCES `waifus` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci
ALTER TABLE waifufightchannels ADD INDEX IX_WaifuFightChannels_WaifuId USING BTREE(WaifuId);
