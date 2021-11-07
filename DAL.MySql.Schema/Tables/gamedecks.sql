CREATE TABLE `gamedecks` (
  `Id` bigint unsigned NOT NULL AUTO_INCREMENT,
  `CTCount` bigint NOT NULL,
  `FavouriteWaifuId` bigint unsigned DEFAULT NULL,
  `Karma` double NOT NULL,
  `ItemsDropped` bigint unsigned NOT NULL,
  `WishlistIsPrivate` tinyint(1) NOT NULL,
  `PVPCoins` bigint NOT NULL,
  `DeckPower` double NOT NULL,
  `PVPWinStreak` bigint NOT NULL,
  `GlobalPVPRank` bigint NOT NULL,
  `SeasonalPVPRank` bigint NOT NULL,
  `MatchMakingRatio` double NOT NULL,
  `PVPDailyGamesPlayed` bigint unsigned NOT NULL,
  `PVPSeasonBeginDate` datetime(6) NOT NULL,
  `ExchangeConditions` varchar(50) DEFAULT NULL,
  `BackgroundImageUrl` varchar(50) DEFAULT NULL,
  `ForegroundImageUrl` varchar(50) DEFAULT NULL,
  `ForegroundColor` varchar(50) DEFAULT NULL,
  `ForegroundPosition` int NOT NULL,
  `BackgroundPosition` int NOT NULL,
  `MaxNumberOfCards` bigint NOT NULL,
  `CardsInGallery` int NOT NULL,
  `UserId` bigint unsigned NOT NULL,
  PRIMARY KEY (`Id`),
  UNIQUE KEY `IX_GameDecks_UserId` (`UserId`),
  KEY `IX_GameDecks_DeckPower` (`DeckPower`),
  CONSTRAINT `FK_GameDecks_Users_UserId` FOREIGN KEY (`UserId`) REFERENCES `users` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci
ALTER TABLE gamedecks ADD INDEX IX_GameDecks_DeckPower USING BTREE(DeckPower);
ALTER TABLE gamedecks ADD UNIQUE INDEX IX_GameDecks_UserId USING BTREE(UserId);