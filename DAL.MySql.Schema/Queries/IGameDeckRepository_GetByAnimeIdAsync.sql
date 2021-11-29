SELECT `g`.`Id`, `g`.`BackgroundImageUrl`, `g`.`BackgroundPosition`, `g`.`CTCount`, `g`.`CardsInGalleryCount`, `g`.`DeckPower`, `g`.`ExchangeConditions`, `g`.`FavouriteWaifuId`, `g`.`ForegroundColor`, `g`.`ForegroundImageUrl`, `g`.`ForegroundPosition`, `g`.`GlobalPVPRank`, `g`.`ItemsDropped`, `g`.`Karma`, `g`.`MatchMakingRatio`, `g`.`MaxNumberOfCards`, `g`.`PVPCoins`, `g`.`PVPDailyGamesPlayed`, `g`.`PVPSeasonBeginDate`, `g`.`PVPWinStreak`, `g`.`SeasonalPVPRank`, `g`.`UserId`, `g`.`WishlistIsPrivate`, `w`.`Id`, `w`.`GameDeckId`, `w`.`ObjectId`, `w`.`ObjectName`, `w`.`Type`
FROM `GameDecks` AS `g`
LEFT JOIN `Wishes` AS `w` ON `g`.`Id` = `w`.`GameDeckId`
WHERE NOT (`g`.`WishlistIsPrivate`) AND EXISTS (
    SELECT 1
    FROM `Wishes` AS `w0`
    WHERE (`g`.`Id` = `w0`.`GameDeckId`) AND ((`w0`.`Type` = 1) AND (`w0`.`ObjectId` = 1)))
ORDER BY `g`.`Id`, `w`.`Id`

