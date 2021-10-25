CREATE TABLE `users` (
  `Id` bigint unsigned NOT NULL AUTO_INCREMENT,
  `ShindenId` bigint unsigned DEFAULT NULL,
  `IsBlacklisted` tinyint(1) NOT NULL,
  `AcCnt` bigint NOT NULL,
  `TcCnt` bigint NOT NULL,
  `ScCnt` bigint NOT NULL,
  `Level` bigint NOT NULL,
  `ExpCnt` bigint NOT NULL,
  `ProfileType` int NOT NULL,
  `BackgroundProfileUri` varchar(50) NOT NULL,
  `StatsReplacementProfileUri` varchar(50) NOT NULL,
  `MessagesCnt` bigint unsigned NOT NULL,
  `CommandsCnt` bigint unsigned NOT NULL,
  `MeasureDate` datetime NOT NULL,
  `MessagesCntAtDate` bigint unsigned NOT NULL,
  `CharacterCntFromDate` bigint unsigned NOT NULL,
  `ShowWaifuInProfile` tinyint(1) NOT NULL,
  `WarningsCount` bigint NOT NULL,
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci