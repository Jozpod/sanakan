CREATE TABLE `penalties` (
  `Id` bigint unsigned NOT NULL AUTO_INCREMENT,
  `UserId` bigint unsigned NOT NULL,
  `GuildId` bigint unsigned NOT NULL,
  `Reason` varchar(100) DEFAULT NULL,
  `Type` int NOT NULL,
  `StartedOn` datetime(6) NOT NULL,
  `Duration` time(6) NOT NULL,
  PRIMARY KEY (`Id`),
  KEY `IX_Penalties_GuildId` (`GuildId`),
  KEY `IX_Penalties_UserId` (`UserId`)
) ENGINE=InnoDB AUTO_INCREMENT=2 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci
ALTER TABLE penalties ADD INDEX IX_Penalties_GuildId USING BTREE(GuildId);
ALTER TABLE penalties ADD INDEX IX_Penalties_UserId USING BTREE(UserId);
