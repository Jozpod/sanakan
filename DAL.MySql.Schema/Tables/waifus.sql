CREATE TABLE `waifus` (
  `Id` bigint unsigned NOT NULL AUTO_INCREMENT,
  `MarketChannelId` bigint unsigned DEFAULT NULL,
  `SpawnChannelId` bigint unsigned DEFAULT NULL,
  `DuelChannelId` bigint unsigned DEFAULT NULL,
  `TrashFightChannelId` bigint unsigned DEFAULT NULL,
  `TrashSpawnChannelId` bigint unsigned DEFAULT NULL,
  `TrashCommandsChannelId` bigint unsigned DEFAULT NULL,
  `GuildOptionsId` bigint unsigned NOT NULL,
  PRIMARY KEY (`Id`),
  UNIQUE KEY `IX_Waifus_GuildOptionsId` (`GuildOptionsId`),
  CONSTRAINT `FK_Waifus_Guilds_GuildOptionsId` FOREIGN KEY (`GuildOptionsId`) REFERENCES `guilds` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci
ALTER TABLE waifus ADD UNIQUE INDEX IX_Waifus_GuildOptionsId USING BTREE(GuildOptionsId);
