CREATE TABLE `waifus` (
  `Id` bigint unsigned NOT NULL AUTO_INCREMENT,
  `MarketChannel` bigint unsigned NOT NULL,
  `SpawnChannel` bigint unsigned NOT NULL,
  `DuelChannel` bigint unsigned NOT NULL,
  `TrashFightChannel` bigint unsigned NOT NULL,
  `TrashSpawnChannel` bigint unsigned NOT NULL,
  `TrashCommandsChannel` bigint unsigned NOT NULL,
  `GuildOptionsId` bigint unsigned NOT NULL,
  PRIMARY KEY (`Id`),
  UNIQUE KEY `IX_Waifus_GuildOptionsId` (`GuildOptionsId`),
  CONSTRAINT `FK_Waifus_Guilds_GuildOptionsId` FOREIGN KEY (`GuildOptionsId`) REFERENCES `guilds` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci