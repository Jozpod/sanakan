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
  `BackgroundImageUrl` varchar(90) DEFAULT NULL,
  `ForegroundImageUrl` varchar(90) DEFAULT NULL,
  `ForegroundColor` varchar(10) DEFAULT NULL,
  `ForegroundPosition` int NOT NULL,
  `BackgroundPosition` int NOT NULL,
  `MaxNumberOfCards` bigint NOT NULL,
  `CardsInGalleryCount` int NOT NULL,
  `UserId` bigint unsigned NOT NULL,
  PRIMARY KEY (`Id`),
  UNIQUE KEY `IX_GameDecks_UserId` (`UserId`),
  KEY `IX_GameDecks_DeckPower` (`DeckPower`),
  KEY `IX_GameDecks_WishlistIsPrivate` (`WishlistIsPrivate`),
  CONSTRAINT `FK_GameDecks_Users_UserId` FOREIGN KEY (`UserId`) REFERENCES `users` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `CK_GameDecks_BackgroundImageUrl` CHECK ((regexp_like(`BackgroundImageUrl`,_utf8mb4'^https?') or (`BackgroundImageUrl` is null))),
  CONSTRAINT `CK_GameDecks_ForegroundColor` CHECK ((regexp_like(`ForegroundColor`,_utf8mb4'^#([a-f0-9]{3}){1,2}$') or (`ForegroundColor` is null))),
  CONSTRAINT `CK_GameDecks_ForegroundImageUrl` CHECK ((regexp_like(`ForegroundImageUrl`,_utf8mb4'^https?') or (`ForegroundImageUrl` is null)))
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci
ALTER TABLE gamedecks ADD INDEX IX_GameDecks_DeckPower USING BTREE(DeckPower);
ALTER TABLE gamedecks ADD UNIQUE INDEX IX_GameDecks_UserId USING BTREE(UserId);
ALTER TABLE gamedecks ADD INDEX IX_GameDecks_WishlistIsPrivate USING BTREE(WishlistIsPrivate);
