SELECT `g`.`Id`, `g`.`BackgroundImageUrl`, `g`.`BackgroundPosition`, `g`.`CTCount`, `g`.`CardsInGalleryCount`, `g`.`DeckPower`, `g`.`ExchangeConditions`, `g`.`FavouriteWaifuId`, `g`.`ForegroundColor`, `g`.`ForegroundImageUrl`, `g`.`ForegroundPosition`, `g`.`GlobalPVPRank`, `g`.`ItemsDropped`, `g`.`Karma`, `g`.`MatchMakingRatio`, `g`.`MaxNumberOfCards`, `g`.`PVPCoins`, `g`.`PVPDailyGamesPlayed`, `g`.`PVPSeasonBeginDate`, `g`.`PVPWinStreak`, `g`.`SeasonalPVPRank`, `g`.`UserId`, `g`.`WishlistIsPrivate`
FROM `GameDecks` AS `g`
WHERE ((`g`.`DeckPower` > 200.0) AND (`g`.`DeckPower` < 800.0)) AND (`g`.`UserId` <> 1)

