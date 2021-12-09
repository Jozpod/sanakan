USE `oldsanakandb`;

BEGIN TRANSACTION

DECLARE QuestionId INT = 1;

INSERT INTO `questions`
(`Id`,
`Group`,
`Answer`,
`PointsWin`,
`PointsLose`,
`Content`,
`TimeToAnswer`)
VALUES
(QuestionId,
<{Group: }>,
<{Answer: }>,
<{PointsWin: }>,
<{PointsLose: }>,
<{Content: }>,
<{TimeToAnswer: }>);


INSERT INTO `answers`
(`Id`,
`Number`,
`Content`,
`QuestionId`)
VALUES
(<{Id: }>,
<{Number: }>,
<{Content: }>,
<{QuestionId: }>);

INSERT INTO `guilds`
(`Id`,
`MuteRole`,
`ModMuteRole`,
`UserRole`,
`AdminRole`,
`GlobalEmotesRole`,
`WaifuRole`,
`NotificationChannel`,
`RaportChannel`,
`QuizChannel`,
`ToDoChannel`,
`NsfwChannel`,
`LogChannel`,
`GreetingChannel`,
`WelcomeMessage`,
`WelcomeMessagePW`,
`GoodbyeMessage`,
`SafariLimit`,
`Supervision`,
`ChaosMode`,
`Prefix`)
VALUES
(<{Id: }>,
<{MuteRole: }>,
<{ModMuteRole: }>,
<{UserRole: }>,
<{AdminRole: }>,
<{GlobalEmotesRole: }>,
<{WaifuRole: }>,
<{NotificationChannel: }>,
<{RaportChannel: }>,
<{QuizChannel: }>,
<{ToDoChannel: }>,
<{NsfwChannel: }>,
<{LogChannel: }>,
<{GreetingChannel: }>,
<{WelcomeMessage: }>,
<{WelcomeMessagePW: }>,
<{GoodbyeMessage: }>,
<{SafariLimit: }>,
<{Supervision: }>,
<{ChaosMode: }>,
<{Prefix: }>);


INSERT INTO `users`
(`Id`,
`Shinden`,
`IsBlacklisted`,
`AcCnt`,
`TcCnt`,
`ScCnt`,
`Level`,
`ExpCnt`,
`ProfileType`,
`BackgroundProfileUri`,
`StatsReplacementProfileUri`,
`MessagesCnt`,
`CommandsCnt`,
`MeasureDate`,
`MessagesCntAtDate`,
`CharacterCntFromDate`,
`ShowWaifuInProfile`,
`Warnings`)
VALUES
(<{Id: }>,
<{Shinden: }>,
<{IsBlacklisted: }>,
<{AcCnt: }>,
<{TcCnt: }>,
<{ScCnt: }>,
<{Level: }>,
<{ExpCnt: }>,
<{ProfileType: }>,
<{BackgroundProfileUri: }>,
<{StatsReplacementProfileUri: }>,
<{MessagesCnt: }>,
<{CommandsCnt: }>,
<{MeasureDate: }>,
<{MessagesCntAtDate: }>,
<{CharacterCntFromDate: }>,
<{ShowWaifuInProfile: }>,
<{Warnings: }>);

INSERT INTO `timestatuses`
(`Id`,
`Type`,
`EndsAt`,
`IValue`,
`BValue`,
`Guild`,
`UserId`)
VALUES
(<{Id: }>,
<{Type: }>,
<{EndsAt: }>,
<{IValue: }>,
<{BValue: }>,
<{Guild: }>,
<{UserId: }>);

INSERT INTO `gamedecks`
(`Id`,
`CTCnt`,
`Waifu`,
`Karma`,
`ItemsDropped`,
`WishlistIsPrivate`,
`PVPCoins`,
`DeckPower`,
`PVPWinStreak`,
`GlobalPVPRank`,
`SeasonalPVPRank`,
`MatachMakingRatio`,
`PVPDailyGamesPlayed`,
`PVPSeasonBeginDate`,
`ExchangeConditions`,
`BackgroundImageUrl`,
`ForegroundImageUrl`,
`ForegroundColor`,
`ForegroundPosition`,
`BackgroundPosition`,
`MaxNumberOfCards`,
`CardsInGallery`,
`UserId`)
VALUES
(<{Id: }>,
<{CTCnt: }>,
<{Waifu: }>,
<{Karma: }>,
<{ItemsDropped: }>,
<{WishlistIsPrivate: }>,
<{PVPCoins: }>,
<{DeckPower: }>,
<{PVPWinStreak: }>,
<{GlobalPVPRank: }>,
<{SeasonalPVPRank: }>,
<{MatachMakingRatio: }>,
<{PVPDailyGamesPlayed: }>,
<{PVPSeasonBeginDate: }>,
<{ExchangeConditions: }>,
<{BackgroundImageUrl: }>,
<{ForegroundImageUrl: }>,
<{ForegroundColor: }>,
<{ForegroundPosition: }>,
<{BackgroundPosition: }>,
<{MaxNumberOfCards: }>,
<{CardsInGallery: }>,
<{UserId: }>);


INSERT INTO `usersstats`
(`Id`,
`ScLost`,
`IncomeInSc`,
`SlotMachineGames`,
`Tail`,
`Head`,
`Hit`,
`Misd`,
`RightAnswers`,
`TotalAnswers`,
`TurnamentsWon`,
`UpgaredCards`,
`SacraficeCards`,
`DestroyedCards`,
`UnleashedCards`,
`ReleasedCards`,
`OpenedBoosterPacks`,
`OpenedBoosterPacksActivity`,
`YamiUpgrades`,
`RaitoUpgrades`,
`YatoUpgrades`,
`WastedTcOnCookies`,
`WastedTcOnCards`,
`UpgradedToSSS`,
`WastedPuzzlesOnCookies`,
`WastedPuzzlesOnCards`,
`UserId`)
VALUES
(<{Id: }>,
<{ScLost: }>,
<{IncomeInSc: }>,
<{SlotMachineGames: }>,
<{Tail: }>,
<{Head: }>,
<{Hit: }>,
<{Misd: }>,
<{RightAnswers: }>,
<{TotalAnswers: }>,
<{TurnamentsWon: }>,
<{UpgaredCards: }>,
<{SacraficeCards: }>,
<{DestroyedCards: }>,
<{UnleashedCards: }>,
<{ReleasedCards: }>,
<{OpenedBoosterPacks: }>,
<{OpenedBoosterPacksActivity: }>,
<{YamiUpgrades: }>,
<{RaitoUpgrades: }>,
<{YatoUpgrades: }>,
<{WastedTcOnCookies: }>,
<{WastedTcOnCards: }>,
<{UpgradedToSSS: }>,
<{WastedPuzzlesOnCookies: }>,
<{WastedPuzzlesOnCards: }>,
<{UserId: }>);



INSERT INTO `cards`
(`Id`,
`Active`,
`InCage`,
`IsTradable`,
`ExpCnt`,
`Affection`,
`UpgradesCnt`,
`RestartCnt`,
`Rarity`,
`RarityOnStart`,
`Dere`,
`Defence`,
`Attack`,
`Health`,
`Name`,
`Character`,
`CreationDate`,
`Source`,
`Title`,
`Image`,
`CustomImage`,
`FirstIdOwner`,
`LastIdOwner`,
`Unique`,
`StarStyle`,
`CustomBorder`,
`MarketValue`,
`Curse`,
`CardPower`,
`EnhanceCnt`,
`FromFigure`,
`Quality`,
`AttackBonus`,
`HealthBonus`,
`DefenceBonus`,
`QualityOnStart`,
`PAS`,
`Expedition`,
`ExpeditionDate`,
`GameDeckId`)
VALUES
(<{Id: }>,
<{Active: }>,
<{InCage: }>,
<{IsTradable: }>,
<{ExpCnt: }>,
<{Affection: }>,
<{UpgradesCnt: }>,
<{RestartCnt: }>,
<{Rarity: }>,
<{RarityOnStart: }>,
<{Dere: }>,
<{Defence: }>,
<{Attack: }>,
<{Health: }>,
<{Name: }>,
<{Character: }>,
<{CreationDate: }>,
<{Source: }>,
<{Title: }>,
<{Image: }>,
<{CustomImage: }>,
<{FirstIdOwner: }>,
<{LastIdOwner: }>,
<{Unique: }>,
<{StarStyle: }>,
<{CustomBorder: }>,
<{MarketValue: }>,
<{Curse: }>,
<{CardPower: }>,
<{EnhanceCnt: }>,
<{FromFigure: }>,
<{Quality: }>,
<{AttackBonus: }>,
<{HealthBonus: }>,
<{DefenceBonus: }>,
<{QualityOnStart: }>,
<{PAS: }>,
<{Expedition: }>,
<{ExpeditionDate: }>,
<{GameDeckId: }>);

INSERT INTO `oldsanakandb`.`figures`
(`Id`,
`Dere`,
`Attack`,
`Health`,
`Defence`,
`Name`,
`Title`,
`IsFocus`,
`ExpCnt`,
`RestartCnt`,
`Character`,
`IsComplete`,
`PAS`,
`SkeletonQuality`,
`CompletionDate`,
`FocusedPart`,
`PartExp`,
`HeadQuality`,
`BodyQuality`,
`LeftArmQuality`,
`RightArmQuality`,
`LeftLegQuality`,
`RightLegQuality`,
`ClothesQuality`,
`GameDeckId`)
VALUES
(<{Id: }>,
<{Dere: }>,
<{Attack: }>,
<{Health: }>,
<{Defence: }>,
<{Name: }>,
<{Title: }>,
<{IsFocus: }>,
<{ExpCnt: }>,
<{RestartCnt: }>,
<{Character: }>,
<{IsComplete: }>,
<{PAS: }>,
<{SkeletonQuality: }>,
<{CompletionDate: }>,
<{FocusedPart: }>,
<{PartExp: }>,
<{HeadQuality: }>,
<{BodyQuality: }>,
<{LeftArmQuality: }>,
<{RightArmQuality: }>,
<{LeftLegQuality: }>,
<{RightLegQuality: }>,
<{ClothesQuality: }>,
<{GameDeckId: }>);

INSERT INTO `cardarenastats`
(`Id`,
`Wins`,
`Loses`,
`Draws`,
`CardId`)
VALUES
(<{Id: }>,
<{Wins: }>,
<{Loses: }>,
<{Draws: }>,
<{CardId: }>);

INSERT INTO `cardpvpstats`
(`Id`,
`Type`,
`Result`,
`GameDeckId`)
VALUES
(<{Id: }>,
<{Type: }>,
<{Result: }>,
<{GameDeckId: }>);

INSERT INTO `cardtags`
(`Id`,
`Name`,
`CardId`)
VALUES
(<{Id: }>,
<{Name: }>,
<{CardId: }>);

INSERT INTO `penalties`
(`Id`,
`User`,
`Guild`,
`Reason`,
`Type`,
`StartDate`,
`DurationInHours`)
VALUES
(<{Id: }>,
<{User: }>,
<{Guild: }>,
<{Reason: }>,
<{Type: }>,
<{StartDate: }>,
<{DurationInHours: }>);

INSERT INTO `commandsdata`
(`Id`,
`UserId`,
`GuildId`,
`Date`,
`CmdName`,
`CmdParams`)
VALUES
(<{Id: }>,
<{UserId: }>,
<{GuildId: }>,
<{Date: }>,
<{CmdName: }>,
<{CmdParams: }>);

INSERT INTO `systemdata`
(`Id`,
`Value`,
`MeasureDate`,
`Type`)
VALUES
(<{Id: }>,
<{Value: }>,
<{MeasureDate: }>,
<{Type: }>);


INSERT INTO `transferdata`
(`Id`,
`Value`,
`Date`,
`DiscordId`,
`ShindenId`,
`Source`)
VALUES
(<{Id: }>,
<{Value: }>,
<{Date: }>,
<{DiscordId: }>,
<{ShindenId: }>,
<{Source: }>);

INSERT INTO `usersdata`
(`Id`,
`Value`,
`UserId`,
`GuildId`,
`MeasureDate`,
`Type`)
VALUES
(<{Id: }>,
<{Value: }>,
<{UserId: }>,
<{GuildId: }>,
<{MeasureDate: }>,
<{Type: }>);

INSERT INTO `boosterpackcharacters`
(`Id`,
`Character`,
`BoosterPackId`)
VALUES
(<{Id: }>,
<{Character: }>,
<{BoosterPackId: }>);

INSERT INTO `boosterpacks`
(`Id`,
`Name`,
`Title`,
`CardCnt`,
`MinRarity`,
`IsCardFromPackTradable`,
`CardSourceFromPack`,
`GameDeckId`)
VALUES
(<{Id: }>,
<{Name: }>,
<{Title: }>,
<{CardCnt: }>,
<{MinRarity: }>,
<{IsCardFromPackTradable: }>,
<{CardSourceFromPack: }>,
<{GameDeckId: }>);

INSERT INTO `slotmachineconfigs`
(`Id`,
`PsayMode`,
`Beat`,
`Rows`,
`Multiplier`,
`UserId`)
VALUES
(<{Id: }>,
<{PsayMode: }>,
<{Beat: }>,
<{Rows: }>,
<{Multiplier: }>,
<{UserId: }>);

INSERT INTO `waifucommandchannels`
(`Id`,
`Channel`,
`WaifuId`)
VALUES
(<{Id: }>,
<{Channel: }>,
<{WaifuId: }>);

INSERT INTO `waifufightchannels`
(`Id`,
`Channel`,
`WaifuId`)
VALUES
(<{Id: }>,
<{Channel: }>,
<{WaifuId: }>);

INSERT INTO `waifus`
(`Id`,
`MarketChannel`,
`SpawnChannel`,
`DuelChannel`,
`TrashFightChannel`,
`TrashSpawnChannel`,
`TrashCommandsChannel`,
`GuildOptionsId`)
VALUES
(<{Id: }>,
<{MarketChannel: }>,
<{SpawnChannel: }>,
<{DuelChannel: }>,
<{TrashFightChannel: }>,
<{TrashSpawnChannel: }>,
<{TrashCommandsChannel: }>,
<{GuildOptionsId: }>);

INSERT INTO `wishes`
(`Id`,
`ObjectId`,
`ObjectName`,
`Type`,
`GameDeckId`)
VALUES
(<{Id: }>,
<{ObjectId: }>,
<{ObjectName: }>,
<{Type: }>,
<{GameDeckId: }>);

INSERT INTO `withoutexpchannels`
(`Id`,
`Channel`,
`GuildOptionsId`)
VALUES
(<{Id: }>,
<{Channel: }>,
<{GuildOptionsId: }>);

INSERT INTO `withoutsupervisionchannels`
(`Id`,
`Channel`,
`GuildOptionsId`)
VALUES
(<{Id: }>,
<{Channel: }>,
<{GuildOptionsId: }>);

INSERT INTO `ignoredchannels`
(`Id`,
`Channel`,
`GuildOptionsId`)
VALUES
(<{Id: }>,
<{Channel: }>,
<{GuildOptionsId: }>);



INSERT INTO `expcontainers`
(`Id`,
`ExpCount`,
`Level`,
`GameDeckId`)
VALUES
(<{Id: }>,
<{ExpCount: }>,
<{Level: }>,
<{GameDeckId: }>);


INSERT INTO `raritysexcludedfrompacks`
(`Id`,
`Rarity`,
`BoosterPackId`)
VALUES
(<{Id: }>,
<{Rarity: }>,
<{BoosterPackId: }>);

INSERT INTO `raports`
(`Id`,
`User`,
`Message`,
`GuildOptionsId`)
VALUES
(<{Id: }>,
<{User: }>,
<{Message: }>,
<{GuildOptionsId: }>);

INSERT INTO `moderatorroles`
(`Id`,
`Role`,
`GuildOptionsId`)
VALUES
(<{Id: }>,
<{Role: }>,
<{GuildOptionsId: }>);

INSERT INTO `levelroles`
(`Id`,
`Role`,
`Level`,
`GuildOptionsId`)
VALUES
(<{Id: }>,
<{Role: }>,
<{Level: }>,
<{GuildOptionsId: }>);

INSERT INTO `ownedroles`
(`Id`,
`Role`,
`PenaltyInfoId`)
VALUES
(<{Id: }>,
<{Role: }>,
<{PenaltyInfoId: }>);

INSERT INTO `selfroles`
(`Id`,
`Role`,
`Name`,
`GuildOptionsId`)
VALUES
(<{Id: }>,
<{Role: }>,
<{Name: }>,
<{GuildOptionsId: }>);

INSERT INTO `mylands`
(`Id`,
`Name`,
`Manager`,
`Underling`,
`GuildOptionsId`)
VALUES
(<{Id: }>,
<{Name: }>,
<{Manager: }>,
<{Underling: }>,
<{GuildOptionsId: }>);


COMMIT TRANSACTION