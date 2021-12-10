# USE `oldsanakandb`;

SET autocommit = 0;

/*
DELETE FROM `questions` WHERE Id = 1;
DELETE FROM `guilds` WHERE Id = 1;
DELETE FROM `users` WHERE Id IN (1, 2);
DELETE FROM `usersstats` WHERE Id IN (1);
# SELECT * FROM `users` WHERE Id IN (1, 2);
*/

DROP PROCEDURE IF EXISTS `SP_Import_Data`;

DELIMITER $$

CREATE PROCEDURE `SP_Import_Data`()
PROC:BEGIN
    DECLARE `_rollback` BOOL DEFAULT 0;
    DECLARE CONTINUE HANDLER FOR SQLEXCEPTION
    BEGIN
		SHOW ERRORS;
		SET `_rollback` = 1;
    END;
    
    SET autocommit = 0;
    
    START TRANSACTION;
    
	SET @QuestionId = 1;
	SET @GuildId = 1;
	SET @User1Id = 1;
	SET @User2Id = 2;
	SET @PenaltyInfo1Id = 1;
	SET @PenaltyInfo2Id = 2;
	SET @CardId = 1;
	SET @GameDeckId = 1;
	SET @BoosterPackId = 1;
	SET @WaifuConfigurationId = 1;

	INSERT INTO `questions`
	(`Id`,
	`Group`,
	`Answer`,
	`PointsWin`,
	`PointsLose`,
	`Content`,
	`TimeToAnswer`)
	VALUES
	(@QuestionId,
	1,
	1,
	1,
	0,
	'test question',
	10);

	INSERT INTO `answers`
	(`Id`,
	`Number`,
	`Content`,
	`QuestionId`)
	VALUES
	(1,
	1,
	'test answer',
	@QuestionId);

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
	(@GuildId, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 'welcome message', 'welcome message private message', 'goodbye message', 50, 0, 0, '.');

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
	(@User1Id, 0, 0, 100, 100, 100, 1, 10, 0, NULL, NULL, 0, 0, CURRENT_DATE(), 0, 0, 0, 0),
	(@User2Id, 0, 0, 100, 100, 100, 1, 10, 0, NULL, NULL, 0, 0, CURRENT_DATE(), 0, 0, 0, 0);

	INSERT INTO `timestatuses`
	(`Id`,
	`Type`,
	`EndsAt`,
	`IValue`,
	`BValue`,
	`Guild`,
	`UserId`)
	VALUES
	(1, 0, CURRENT_DATE(), 1, 0, 0, @User1Id);

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
	(1, 100, 1, 1, 1, 0, 100, 10, 1, 1, 1, 1, 1, CURRENT_DATE(), 0, NULL, NULL, '#FFFFFF', 0, 0, 10, 1, @User1Id);

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
	(1, 100, 1000, 0, 0, 0, 0, 0, 0, 0, 0, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, @User1Id);

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
	(@CardId, 0, 0, 1, 0, 100, 0, 0, 0, 0, 0, 100, 50, 50, 'test card', 1, 
	CURRENT_DATE(), 0, 'title', 'image', 'custom', 1, 0, 0, 0, 'test', 1, 100,
	0, 0, 0, 0, 0, 0, 50, 50, 50, 1, CURRENT_DATE(), @GameDeckId);

	INSERT INTO `figures`
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
	(1, 0, 100, 50, 50, 'figure', 'title', 0, 100, 10, 1, 0, 1, 1, 0, 1, CURRENT_DATE(), 1, 1, 1, 1, 0, 0 , 1, @GameDeckId);

	INSERT INTO `cardarenastats`
	(`Id`,
	`Wins`,
	`Loses`,
	`Draws`,
	`CardId`)
	VALUES
	(1, 10, 0, 5, @CardId);

	INSERT INTO `cardpvpstats`
	(`Id`,
	`Type`,
	`Result`,
	`GameDeckId`)
	VALUES
	(1, 0, 1, @GameDeckId);

	INSERT INTO `cardtags`
	(`Id`,
	`Name`,
	`CardId`)
	VALUES
	(1, 'test tag', @CardId);
    
	INSERT INTO `penalties`
	(`Id`,
	`User`,
	`Guild`,
	`Reason`,
	`Type`,
	`StartDate`,
	`DurationInHours`)
	VALUES
	(@PenaltyInfo1Id, 1, 1, 'test reason', 0, CURRENT_DATE(), 100),
	(@PenaltyInfo2Id, 1, 1, 'test reason', 0, CURRENT_DATE(), 100);

	INSERT INTO `commandsdata`
	(`Id`,
	`UserId`,
	`GuildId`,
	`Date`,
	`CmdName`,
	`CmdParams`)
	VALUES
	(1, @User1Id, 1, CURRENT_DATE(), 'test', 'test');
    
	INSERT INTO `systemdata`
	(`Id`,
	`Value`,
	`MeasureDate`,
	`Type`)
	VALUES
	(1, 1, CURRENT_DATE(), 1);

	INSERT INTO `transferdata`
	(`Id`,
	`Value`,
	`Date`,
	`DiscordId`,
	`ShindenId`,
	`Source`)
	VALUES
	(1, 1, CURRENT_DATE(), 1, 1, 0);

	INSERT INTO `usersdata`
	(`Id`,
	`Value`,
	`UserId`,
	`GuildId`,
	`MeasureDate`,
	`Type`)
	VALUES
	(1, 1, @User1Id, 0, CURRENT_DATE(), 1);

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
	(@BoosterPackId, 'test', 1, 1, 0, 1, 0, @GameDeckId);

	INSERT INTO `boosterpackcharacters`
	(`Id`,
	`Character`,
	`BoosterPackId`)
	VALUES
	(1, 1, @BoosterPackId);

	INSERT INTO `slotmachineconfigs`
	(`Id`,
	`PsayMode`,
	`Beat`,
	`Rows`,
	`Multiplier`,
	`UserId`)
	VALUES
	(1, 0, 1, 10, 1, @User1Id);

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
	(@WaifuConfigurationId, 1, 1, 1, 1, 1, 1, @GuildId);

	INSERT INTO `waifucommandchannels`
	(`Id`,
	`Channel`,
	`WaifuId`)
	VALUES
	(1, 1, @WaifuConfigurationId);

	INSERT INTO `waifufightchannels`
	(`Id`,
	`Channel`,
	`WaifuId`)
	VALUES
	(1, 1, @WaifuConfigurationId);

	INSERT INTO `wishes`
	(`Id`,
	`ObjectId`,
	`ObjectName`,
	`Type`,
	`GameDeckId`)
	VALUES
	(1, 1, 'test item', 0, @GameDeckId);

	INSERT INTO `withoutexpchannels`
	(`Id`,
	`Channel`,
	`GuildOptionsId`)
	VALUES
	(1, 1, @GuildId);

	INSERT INTO `withoutsupervisionchannels`
	(`Id`,
	`Channel`,
	`GuildOptionsId`)
	VALUES
	(1, 1, @GuildId);

	INSERT INTO `ignoredchannels`
	(`Id`,
	`Channel`,
	`GuildOptionsId`)
	VALUES
	(1, 1, @GuildId);

	INSERT INTO `expcontainers`
	(`Id`,
	`ExpCount`,
	`Level`,
	`GameDeckId`)
	VALUES
	(1, 100, 1, @GameDeckId);

	INSERT INTO `raritysexcludedfrompacks`
	(`Id`,
	`Rarity`,
	`BoosterPackId`)
	VALUES
	(1, 1, 1);

	INSERT INTO `raports`
	(`Id`,
	`User`,
	`Message`,
	`GuildOptionsId`)
	VALUES
	(1, @User1Id, 1, @GuildId);

	INSERT INTO `moderatorroles`
	(`Id`,
	`Role`,
	`GuildOptionsId`)
	VALUES
	(1, 1, @GuildId);

	INSERT INTO `levelroles`
	(`Id`,
	`Role`,
	`Level`,
	`GuildOptionsId`)
	VALUES
	(1, 1, 1, @GuildId);

	INSERT INTO `ownedroles`
	(`Id`,
	`Role`,
	`PenaltyInfoId`)
	VALUES
	(1, 1, @PenaltyInfo2Id);

	INSERT INTO `selfroles`
	(`Id`,
	`Role`,
	`Name`,
	`GuildOptionsId`)
	VALUES
	(1, 1, 'test self role', @GuildId);

	INSERT INTO `mylands`
	(`Id`,
	`Name`,
	`Manager`,
	`Underling`,
	`GuildOptionsId`)
	VALUES
	(1, 'test land' , 1, 1, @GuildId);
    
	# ROLLBACK;
	# COMMIT;
    
    IF `_rollback` THEN
        ROLLBACK;
    ELSE
        COMMIT;
    END IF;
END$$

DELIMITER ;

CALL `SP_Import_Data`;