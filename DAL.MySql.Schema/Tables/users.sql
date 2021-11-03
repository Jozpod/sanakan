CREATE TABLE `users` (
  `Id` bigint unsigned NOT NULL AUTO_INCREMENT,
  `ShindenId` bigint unsigned DEFAULT NULL,
  `IsBlacklisted` tinyint(1) NOT NULL,
  `AcCount` bigint NOT NULL,
  `TcCount` bigint NOT NULL,
  `ScCount` bigint NOT NULL,
  `Level` bigint unsigned NOT NULL,
  `ExperienceCount` bigint unsigned NOT NULL,
  `ProfileType` int NOT NULL,
  `BackgroundProfileUri` varchar(50) NOT NULL,
  `StatsReplacementProfileUri` varchar(50) NOT NULL,
  `MessagesCount` bigint unsigned NOT NULL,
  `CommandsCount` bigint unsigned NOT NULL,
  `MeasureDate` datetime(6) NOT NULL,
  `MessagesCountAtDate` bigint unsigned NOT NULL,
  `CharacterCountFromDate` bigint unsigned NOT NULL,
  `ShowWaifuInProfile` tinyint(1) NOT NULL,
  `WarningsCount` bigint NOT NULL,
  PRIMARY KEY (`Id`),
  KEY `IX_Users_ShindenId` (`ShindenId`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci
ALTER TABLE users ADD INDEX IX_Users_ShindenId USING BTREE(ShindenId);
ALTER TABLE users ADD UNIQUE INDEX USER USING HASH(USER);
