CREATE DATABASE `oldsanakandb` /*!40100 DEFAULT CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci */ /*!80016 DEFAULT ENCRYPTION='N' */;

USE `oldsanakandb`;

/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;
/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;
/*!50503 SET NAMES utf8 */;
/*!40103 SET @OLD_TIME_ZONE=@@TIME_ZONE */;
/*!40103 SET TIME_ZONE='+00:00' */;
/*!40014 SET @OLD_UNIQUE_CHECKS=@@UNIQUE_CHECKS, UNIQUE_CHECKS=0 */;
/*!40014 SET @OLD_FOREIGN_KEY_CHECKS=@@FOREIGN_KEY_CHECKS, FOREIGN_KEY_CHECKS=0 */;
/*!40101 SET @OLD_SQL_MODE=@@SQL_MODE, SQL_MODE='NO_AUTO_VALUE_ON_ZERO' */;
/*!40111 SET @OLD_SQL_NOTES=@@SQL_NOTES, SQL_NOTES=0 */;

--
-- Table structure for table `answers`
--

DROP TABLE IF EXISTS `answers`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `answers` (
  `Id` bigint unsigned NOT NULL AUTO_INCREMENT,
  `Number` int NOT NULL,
  `Content` longtext,
  `QuestionId` bigint unsigned NOT NULL,
  PRIMARY KEY (`Id`),
  KEY `IX_Answers_QuestionId` (`QuestionId`),
  CONSTRAINT `FK_Answers_Questions_QuestionId` FOREIGN KEY (`QuestionId`) REFERENCES `questions` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `boosterpackcharacters`
--

DROP TABLE IF EXISTS `boosterpackcharacters`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `boosterpackcharacters` (
  `Id` bigint unsigned NOT NULL AUTO_INCREMENT,
  `Character` bigint unsigned NOT NULL,
  `BoosterPackId` bigint unsigned NOT NULL,
  PRIMARY KEY (`Id`),
  KEY `IX_BoosterPackCharacters_BoosterPackId` (`BoosterPackId`),
  CONSTRAINT `FK_BoosterPackCharacters_BoosterPacks_BoosterPackId` FOREIGN KEY (`BoosterPackId`) REFERENCES `boosterpacks` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `boosterpacks`
--

DROP TABLE IF EXISTS `boosterpacks`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `boosterpacks` (
  `Id` bigint unsigned NOT NULL AUTO_INCREMENT,
  `Name` longtext,
  `Title` bigint unsigned NOT NULL,
  `CardCnt` int NOT NULL,
  `MinRarity` int NOT NULL,
  `IsCardFromPackTradable` tinyint(1) NOT NULL,
  `CardSourceFromPack` int NOT NULL,
  `GameDeckId` bigint unsigned NOT NULL,
  PRIMARY KEY (`Id`),
  KEY `IX_BoosterPacks_GameDeckId` (`GameDeckId`),
  CONSTRAINT `FK_BoosterPacks_GameDecks_GameDeckId` FOREIGN KEY (`GameDeckId`) REFERENCES `gamedecks` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `cardarenastats`
--

DROP TABLE IF EXISTS `cardarenastats`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `cardarenastats` (
  `Id` bigint unsigned NOT NULL AUTO_INCREMENT,
  `Wins` bigint NOT NULL,
  `Loses` bigint NOT NULL,
  `Draws` bigint NOT NULL,
  `CardId` bigint unsigned NOT NULL,
  PRIMARY KEY (`Id`),
  UNIQUE KEY `IX_CardArenaStats_CardId` (`CardId`),
  CONSTRAINT `FK_CardArenaStats_Cards_CardId` FOREIGN KEY (`CardId`) REFERENCES `cards` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `cardpvpstats`
--

DROP TABLE IF EXISTS `cardpvpstats`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `cardpvpstats` (
  `Id` bigint unsigned NOT NULL AUTO_INCREMENT,
  `Type` int NOT NULL,
  `Result` int NOT NULL,
  `GameDeckId` bigint unsigned NOT NULL,
  PRIMARY KEY (`Id`),
  KEY `IX_CardPvPStats_GameDeckId` (`GameDeckId`),
  CONSTRAINT `FK_CardPvPStats_GameDecks_GameDeckId` FOREIGN KEY (`GameDeckId`) REFERENCES `gamedecks` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `cards`
--

DROP TABLE IF EXISTS `cards`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `cards` (
  `Id` bigint unsigned NOT NULL AUTO_INCREMENT,
  `Active` tinyint(1) NOT NULL,
  `InCage` tinyint(1) NOT NULL,
  `IsTradable` tinyint(1) NOT NULL,
  `ExpCnt` double NOT NULL,
  `Affection` double NOT NULL,
  `UpgradesCnt` int NOT NULL,
  `RestartCnt` int NOT NULL,
  `Rarity` int NOT NULL,
  `RarityOnStart` int NOT NULL,
  `Dere` int NOT NULL,
  `Defence` int NOT NULL,
  `Attack` int NOT NULL,
  `Health` int NOT NULL,
  `Name` longtext,
  `Character` bigint unsigned NOT NULL,
  `CreationDate` datetime(6) NOT NULL,
  `Source` int NOT NULL,
  `Title` longtext,
  `Image` longtext,
  `CustomImage` longtext,
  `FirstIdOwner` bigint unsigned NOT NULL,
  `LastIdOwner` bigint unsigned NOT NULL,
  `Unique` tinyint(1) NOT NULL,
  `StarStyle` int NOT NULL,
  `CustomBorder` longtext,
  `MarketValue` double NOT NULL,
  `Curse` int NOT NULL,
  `CardPower` double NOT NULL,
  `EnhanceCnt` int NOT NULL,
  `FromFigure` tinyint(1) NOT NULL,
  `Quality` int NOT NULL,
  `AttackBonus` int NOT NULL,
  `HealthBonus` int NOT NULL,
  `DefenceBonus` int NOT NULL,
  `QualityOnStart` int NOT NULL,
  `PAS` int NOT NULL,
  `Expedition` int NOT NULL,
  `ExpeditionDate` datetime(6) NOT NULL,
  `GameDeckId` bigint unsigned NOT NULL,
  PRIMARY KEY (`Id`),
  KEY `IX_Cards_Active` (`Active`),
  KEY `IX_Cards_GameDeckId` (`GameDeckId`),
  CONSTRAINT `FK_Cards_GameDecks_GameDeckId` FOREIGN KEY (`GameDeckId`) REFERENCES `gamedecks` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `cardtags`
--

DROP TABLE IF EXISTS `cardtags`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `cardtags` (
  `Id` bigint unsigned NOT NULL AUTO_INCREMENT,
  `Name` longtext,
  `CardId` bigint unsigned NOT NULL,
  PRIMARY KEY (`Id`),
  KEY `IX_CardTags_CardId` (`CardId`),
  CONSTRAINT `FK_CardTags_Cards_CardId` FOREIGN KEY (`CardId`) REFERENCES `cards` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `commandchannels`
--

DROP TABLE IF EXISTS `commandchannels`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `commandchannels` (
  `Id` bigint unsigned NOT NULL AUTO_INCREMENT,
  `Channel` bigint unsigned NOT NULL,
  `GuildOptionsId` bigint unsigned NOT NULL,
  PRIMARY KEY (`Id`),
  KEY `IX_CommandChannels_GuildOptionsId` (`GuildOptionsId`),
  CONSTRAINT `FK_CommandChannels_Guilds_GuildOptionsId` FOREIGN KEY (`GuildOptionsId`) REFERENCES `guilds` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `commandsdata`
--

DROP TABLE IF EXISTS `commandsdata`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `commandsdata` (
  `Id` bigint unsigned NOT NULL AUTO_INCREMENT,
  `UserId` bigint unsigned NOT NULL,
  `GuildId` bigint unsigned NOT NULL,
  `Date` datetime(6) NOT NULL,
  `CmdName` longtext,
  `CmdParams` longtext,
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `expcontainers`
--

DROP TABLE IF EXISTS `expcontainers`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `expcontainers` (
  `Id` bigint unsigned NOT NULL AUTO_INCREMENT,
  `ExpCount` double NOT NULL,
  `Level` int NOT NULL,
  `GameDeckId` bigint unsigned NOT NULL,
  PRIMARY KEY (`Id`),
  UNIQUE KEY `IX_ExpContainers_GameDeckId` (`GameDeckId`),
  CONSTRAINT `FK_ExpContainers_GameDecks_GameDeckId` FOREIGN KEY (`GameDeckId`) REFERENCES `gamedecks` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `figures`
--

DROP TABLE IF EXISTS `figures`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `figures` (
  `Id` bigint unsigned NOT NULL AUTO_INCREMENT,
  `Dere` int NOT NULL,
  `Attack` int NOT NULL,
  `Health` int NOT NULL,
  `Defence` int NOT NULL,
  `Name` longtext,
  `Title` longtext,
  `IsFocus` tinyint(1) NOT NULL,
  `ExpCnt` double NOT NULL,
  `RestartCnt` int NOT NULL,
  `Character` bigint unsigned NOT NULL,
  `IsComplete` tinyint(1) NOT NULL,
  `PAS` int NOT NULL,
  `SkeletonQuality` int NOT NULL,
  `CompletionDate` datetime(6) NOT NULL,
  `FocusedPart` int NOT NULL,
  `PartExp` double NOT NULL,
  `HeadQuality` int NOT NULL,
  `BodyQuality` int NOT NULL,
  `LeftArmQuality` int NOT NULL,
  `RightArmQuality` int NOT NULL,
  `LeftLegQuality` int NOT NULL,
  `RightLegQuality` int NOT NULL,
  `ClothesQuality` int NOT NULL,
  `GameDeckId` bigint unsigned NOT NULL,
  PRIMARY KEY (`Id`),
  KEY `IX_Figures_GameDeckId` (`GameDeckId`),
  CONSTRAINT `FK_Figures_GameDecks_GameDeckId` FOREIGN KEY (`GameDeckId`) REFERENCES `gamedecks` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `gamedecks`
--

DROP TABLE IF EXISTS `gamedecks`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `gamedecks` (
  `Id` bigint unsigned NOT NULL AUTO_INCREMENT,
  `CTCnt` bigint NOT NULL,
  `Waifu` bigint unsigned NOT NULL,
  `Karma` double NOT NULL,
  `ItemsDropped` bigint unsigned NOT NULL,
  `WishlistIsPrivate` tinyint(1) NOT NULL,
  `PVPCoins` bigint NOT NULL,
  `DeckPower` double NOT NULL,
  `PVPWinStreak` bigint NOT NULL,
  `GlobalPVPRank` bigint NOT NULL,
  `SeasonalPVPRank` bigint NOT NULL,
  `MatachMakingRatio` double NOT NULL,
  `PVPDailyGamesPlayed` bigint unsigned NOT NULL,
  `PVPSeasonBeginDate` datetime(6) NOT NULL,
  `ExchangeConditions` longtext,
  `BackgroundImageUrl` longtext,
  `ForegroundImageUrl` longtext,
  `ForegroundColor` longtext,
  `ForegroundPosition` int NOT NULL,
  `BackgroundPosition` int NOT NULL,
  `MaxNumberOfCards` bigint NOT NULL,
  `CardsInGallery` int NOT NULL,
  `UserId` bigint unsigned NOT NULL,
  PRIMARY KEY (`Id`),
  UNIQUE KEY `IX_GameDecks_UserId` (`UserId`),
  KEY `IX_GameDecks_DeckPower` (`DeckPower`),
  CONSTRAINT `FK_GameDecks_Users_UserId` FOREIGN KEY (`UserId`) REFERENCES `users` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `guilds`
--

DROP TABLE IF EXISTS `guilds`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `guilds` (
  `Id` bigint unsigned NOT NULL AUTO_INCREMENT,
  `MuteRole` bigint unsigned NOT NULL,
  `ModMuteRole` bigint unsigned NOT NULL,
  `UserRole` bigint unsigned NOT NULL,
  `AdminRole` bigint unsigned NOT NULL,
  `GlobalEmotesRole` bigint unsigned NOT NULL,
  `WaifuRole` bigint unsigned NOT NULL,
  `NotificationChannel` bigint unsigned NOT NULL,
  `RaportChannel` bigint unsigned NOT NULL,
  `QuizChannel` bigint unsigned NOT NULL,
  `ToDoChannel` bigint unsigned NOT NULL,
  `NsfwChannel` bigint unsigned NOT NULL,
  `LogChannel` bigint unsigned NOT NULL,
  `GreetingChannel` bigint unsigned NOT NULL,
  `WelcomeMessage` longtext,
  `WelcomeMessagePW` longtext,
  `GoodbyeMessage` longtext,
  `SafariLimit` bigint NOT NULL,
  `Supervision` tinyint(1) NOT NULL,
  `ChaosMode` tinyint(1) NOT NULL,
  `Prefix` longtext,
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `ignoredchannels`
--

DROP TABLE IF EXISTS `ignoredchannels`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `ignoredchannels` (
  `Id` bigint unsigned NOT NULL AUTO_INCREMENT,
  `Channel` bigint unsigned NOT NULL,
  `GuildOptionsId` bigint unsigned NOT NULL,
  PRIMARY KEY (`Id`),
  KEY `IX_IgnoredChannels_GuildOptionsId` (`GuildOptionsId`),
  CONSTRAINT `FK_IgnoredChannels_Guilds_GuildOptionsId` FOREIGN KEY (`GuildOptionsId`) REFERENCES `guilds` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `items`
--

DROP TABLE IF EXISTS `items`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `items` (
  `Id` bigint unsigned NOT NULL AUTO_INCREMENT,
  `Count` bigint NOT NULL,
  `Name` longtext,
  `Type` int NOT NULL,
  `Quality` int NOT NULL,
  `GameDeckId` bigint unsigned NOT NULL,
  PRIMARY KEY (`Id`),
  KEY `IX_Items_GameDeckId` (`GameDeckId`),
  CONSTRAINT `FK_Items_GameDecks_GameDeckId` FOREIGN KEY (`GameDeckId`) REFERENCES `gamedecks` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `levelroles`
--

DROP TABLE IF EXISTS `levelroles`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `levelroles` (
  `Id` bigint unsigned NOT NULL AUTO_INCREMENT,
  `Role` bigint unsigned NOT NULL,
  `Level` bigint unsigned NOT NULL,
  `GuildOptionsId` bigint unsigned NOT NULL,
  PRIMARY KEY (`Id`),
  KEY `IX_LevelRoles_GuildOptionsId` (`GuildOptionsId`),
  CONSTRAINT `FK_LevelRoles_Guilds_GuildOptionsId` FOREIGN KEY (`GuildOptionsId`) REFERENCES `guilds` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `moderatorroles`
--

DROP TABLE IF EXISTS `moderatorroles`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `moderatorroles` (
  `Id` bigint unsigned NOT NULL AUTO_INCREMENT,
  `Role` bigint unsigned NOT NULL,
  `GuildOptionsId` bigint unsigned NOT NULL,
  PRIMARY KEY (`Id`),
  KEY `IX_ModeratorRoles_GuildOptionsId` (`GuildOptionsId`),
  CONSTRAINT `FK_ModeratorRoles_Guilds_GuildOptionsId` FOREIGN KEY (`GuildOptionsId`) REFERENCES `guilds` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `mylands`
--

DROP TABLE IF EXISTS `mylands`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `mylands` (
  `Id` bigint unsigned NOT NULL AUTO_INCREMENT,
  `Name` longtext,
  `Manager` bigint unsigned NOT NULL,
  `Underling` bigint unsigned NOT NULL,
  `GuildOptionsId` bigint unsigned NOT NULL,
  PRIMARY KEY (`Id`),
  KEY `IX_MyLands_GuildOptionsId` (`GuildOptionsId`),
  CONSTRAINT `FK_MyLands_Guilds_GuildOptionsId` FOREIGN KEY (`GuildOptionsId`) REFERENCES `guilds` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `ownedroles`
--

DROP TABLE IF EXISTS `ownedroles`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `ownedroles` (
  `Id` bigint unsigned NOT NULL AUTO_INCREMENT,
  `Role` bigint unsigned NOT NULL,
  `PenaltyInfoId` bigint unsigned NOT NULL,
  PRIMARY KEY (`Id`),
  KEY `IX_OwnedRoles_PenaltyInfoId` (`PenaltyInfoId`),
  CONSTRAINT `FK_OwnedRoles_Penalties_PenaltyInfoId` FOREIGN KEY (`PenaltyInfoId`) REFERENCES `penalties` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `penalties`
--

DROP TABLE IF EXISTS `penalties`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `penalties` (
  `Id` bigint unsigned NOT NULL AUTO_INCREMENT,
  `User` bigint unsigned NOT NULL,
  `Guild` bigint unsigned NOT NULL,
  `Reason` longtext,
  `Type` int NOT NULL,
  `StartDate` datetime(6) NOT NULL,
  `DurationInHours` bigint NOT NULL,
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `questions`
--

DROP TABLE IF EXISTS `questions`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `questions` (
  `Id` bigint unsigned NOT NULL AUTO_INCREMENT,
  `Group` int NOT NULL,
  `Answer` int NOT NULL,
  `PointsWin` int NOT NULL,
  `PointsLose` int NOT NULL,
  `Content` longtext,
  `TimeToAnswer` int NOT NULL,
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `raports`
--

DROP TABLE IF EXISTS `raports`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `raports` (
  `Id` bigint unsigned NOT NULL AUTO_INCREMENT,
  `User` bigint unsigned NOT NULL,
  `Message` bigint unsigned NOT NULL,
  `GuildOptionsId` bigint unsigned NOT NULL,
  PRIMARY KEY (`Id`),
  KEY `IX_Raports_GuildOptionsId` (`GuildOptionsId`),
  CONSTRAINT `FK_Raports_Guilds_GuildOptionsId` FOREIGN KEY (`GuildOptionsId`) REFERENCES `guilds` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `raritysexcludedfrompacks`
--

DROP TABLE IF EXISTS `raritysexcludedfrompacks`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `raritysexcludedfrompacks` (
  `Id` bigint unsigned NOT NULL AUTO_INCREMENT,
  `Rarity` int NOT NULL,
  `BoosterPackId` bigint unsigned NOT NULL,
  PRIMARY KEY (`Id`),
  KEY `IX_RaritysExcludedFromPacks_BoosterPackId` (`BoosterPackId`),
  CONSTRAINT `FK_RaritysExcludedFromPacks_BoosterPacks_BoosterPackId` FOREIGN KEY (`BoosterPackId`) REFERENCES `boosterpacks` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `selfroles`
--

DROP TABLE IF EXISTS `selfroles`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `selfroles` (
  `Id` bigint unsigned NOT NULL AUTO_INCREMENT,
  `Role` bigint unsigned NOT NULL,
  `Name` longtext,
  `GuildOptionsId` bigint unsigned NOT NULL,
  PRIMARY KEY (`Id`),
  KEY `IX_SelfRoles_GuildOptionsId` (`GuildOptionsId`),
  CONSTRAINT `FK_SelfRoles_Guilds_GuildOptionsId` FOREIGN KEY (`GuildOptionsId`) REFERENCES `guilds` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `slotmachineconfigs`
--

DROP TABLE IF EXISTS `slotmachineconfigs`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `slotmachineconfigs` (
  `Id` bigint unsigned NOT NULL AUTO_INCREMENT,
  `PsayMode` bigint NOT NULL,
  `Beat` int NOT NULL,
  `Rows` int NOT NULL,
  `Multiplier` int NOT NULL,
  `UserId` bigint unsigned NOT NULL,
  PRIMARY KEY (`Id`),
  UNIQUE KEY `IX_SlotMachineConfigs_UserId` (`UserId`),
  CONSTRAINT `FK_SlotMachineConfigs_Users_UserId` FOREIGN KEY (`UserId`) REFERENCES `users` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `systemdata`
--

DROP TABLE IF EXISTS `systemdata`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `systemdata` (
  `Id` bigint unsigned NOT NULL AUTO_INCREMENT,
  `Value` bigint NOT NULL,
  `MeasureDate` datetime(6) NOT NULL,
  `Type` int NOT NULL,
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `timestatuses`
--

DROP TABLE IF EXISTS `timestatuses`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `timestatuses` (
  `Id` bigint unsigned NOT NULL AUTO_INCREMENT,
  `Type` int NOT NULL,
  `EndsAt` datetime(6) NOT NULL,
  `IValue` bigint NOT NULL,
  `BValue` tinyint(1) NOT NULL,
  `Guild` bigint unsigned NOT NULL,
  `UserId` bigint unsigned NOT NULL,
  PRIMARY KEY (`Id`),
  KEY `IX_TimeStatuses_UserId` (`UserId`),
  CONSTRAINT `FK_TimeStatuses_Users_UserId` FOREIGN KEY (`UserId`) REFERENCES `users` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `transferdata`
--

DROP TABLE IF EXISTS `transferdata`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `transferdata` (
  `Id` bigint unsigned NOT NULL AUTO_INCREMENT,
  `Value` bigint NOT NULL,
  `Date` datetime(6) NOT NULL,
  `DiscordId` bigint unsigned NOT NULL,
  `ShindenId` bigint unsigned NOT NULL,
  `Source` int NOT NULL,
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `users`
--

DROP TABLE IF EXISTS `users`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `users` (
  `Id` bigint unsigned NOT NULL AUTO_INCREMENT,
  `Shinden` bigint unsigned NOT NULL,
  `IsBlacklisted` tinyint(1) NOT NULL,
  `AcCnt` bigint NOT NULL,
  `TcCnt` bigint NOT NULL,
  `ScCnt` bigint NOT NULL,
  `Level` bigint NOT NULL,
  `ExpCnt` bigint NOT NULL,
  `ProfileType` int NOT NULL,
  `BackgroundProfileUri` longtext,
  `StatsReplacementProfileUri` longtext,
  `MessagesCnt` bigint unsigned NOT NULL,
  `CommandsCnt` bigint unsigned NOT NULL,
  `MeasureDate` datetime(6) NOT NULL,
  `MessagesCntAtDate` bigint unsigned NOT NULL,
  `CharacterCntFromDate` bigint unsigned NOT NULL,
  `ShowWaifuInProfile` tinyint(1) NOT NULL,
  `Warnings` bigint NOT NULL,
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `usersdata`
--

DROP TABLE IF EXISTS `usersdata`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `usersdata` (
  `Id` bigint unsigned NOT NULL AUTO_INCREMENT,
  `Value` bigint NOT NULL,
  `UserId` bigint unsigned NOT NULL,
  `GuildId` bigint unsigned NOT NULL,
  `MeasureDate` datetime(6) NOT NULL,
  `Type` int NOT NULL,
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `usersstats`
--

DROP TABLE IF EXISTS `usersstats`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `usersstats` (
  `Id` bigint unsigned NOT NULL AUTO_INCREMENT,
  `ScLost` bigint NOT NULL,
  `IncomeInSc` bigint NOT NULL,
  `SlotMachineGames` bigint NOT NULL,
  `Tail` bigint NOT NULL,
  `Head` bigint NOT NULL,
  `Hit` bigint NOT NULL,
  `Misd` bigint NOT NULL,
  `RightAnswers` bigint NOT NULL,
  `TotalAnswers` bigint NOT NULL,
  `TurnamentsWon` bigint NOT NULL,
  `UpgaredCards` bigint NOT NULL,
  `SacraficeCards` bigint NOT NULL,
  `DestroyedCards` bigint NOT NULL,
  `UnleashedCards` bigint NOT NULL,
  `ReleasedCards` bigint NOT NULL,
  `OpenedBoosterPacks` bigint NOT NULL,
  `OpenedBoosterPacksActivity` bigint NOT NULL,
  `YamiUpgrades` bigint NOT NULL,
  `RaitoUpgrades` bigint NOT NULL,
  `YatoUpgrades` bigint NOT NULL,
  `WastedTcOnCookies` bigint NOT NULL,
  `WastedTcOnCards` bigint NOT NULL,
  `UpgradedToSSS` bigint NOT NULL,
  `WastedPuzzlesOnCookies` bigint NOT NULL,
  `WastedPuzzlesOnCards` bigint NOT NULL,
  `UserId` bigint unsigned NOT NULL,
  PRIMARY KEY (`Id`),
  UNIQUE KEY `IX_UsersStats_UserId` (`UserId`),
  CONSTRAINT `FK_UsersStats_Users_UserId` FOREIGN KEY (`UserId`) REFERENCES `users` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `waifucommandchannels`
--

DROP TABLE IF EXISTS `waifucommandchannels`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `waifucommandchannels` (
  `Id` bigint unsigned NOT NULL AUTO_INCREMENT,
  `Channel` bigint unsigned NOT NULL,
  `WaifuId` bigint unsigned NOT NULL,
  PRIMARY KEY (`Id`),
  KEY `IX_WaifuCommandChannels_WaifuId` (`WaifuId`),
  CONSTRAINT `FK_WaifuCommandChannels_Waifus_WaifuId` FOREIGN KEY (`WaifuId`) REFERENCES `waifus` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `waifufightchannels`
--

DROP TABLE IF EXISTS `waifufightchannels`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `waifufightchannels` (
  `Id` bigint unsigned NOT NULL AUTO_INCREMENT,
  `Channel` bigint unsigned NOT NULL,
  `WaifuId` bigint unsigned NOT NULL,
  PRIMARY KEY (`Id`),
  KEY `IX_WaifuFightChannels_WaifuId` (`WaifuId`),
  CONSTRAINT `FK_WaifuFightChannels_Waifus_WaifuId` FOREIGN KEY (`WaifuId`) REFERENCES `waifus` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `waifus`
--

DROP TABLE IF EXISTS `waifus`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
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
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `wishes`
--

DROP TABLE IF EXISTS `wishes`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `wishes` (
  `Id` bigint unsigned NOT NULL AUTO_INCREMENT,
  `ObjectId` bigint unsigned NOT NULL,
  `ObjectName` longtext,
  `Type` int NOT NULL,
  `GameDeckId` bigint unsigned NOT NULL,
  PRIMARY KEY (`Id`),
  KEY `IX_Wishes_GameDeckId` (`GameDeckId`),
  CONSTRAINT `FK_Wishes_GameDecks_GameDeckId` FOREIGN KEY (`GameDeckId`) REFERENCES `gamedecks` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `withoutexpchannels`
--

DROP TABLE IF EXISTS `withoutexpchannels`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `withoutexpchannels` (
  `Id` bigint unsigned NOT NULL AUTO_INCREMENT,
  `Channel` bigint unsigned NOT NULL,
  `GuildOptionsId` bigint unsigned NOT NULL,
  PRIMARY KEY (`Id`),
  KEY `IX_WithoutExpChannels_GuildOptionsId` (`GuildOptionsId`),
  CONSTRAINT `FK_WithoutExpChannels_Guilds_GuildOptionsId` FOREIGN KEY (`GuildOptionsId`) REFERENCES `guilds` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `withoutsupervisionchannels`
--

DROP TABLE IF EXISTS `withoutsupervisionchannels`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `withoutsupervisionchannels` (
  `Id` bigint unsigned NOT NULL AUTO_INCREMENT,
  `Channel` bigint unsigned NOT NULL,
  `GuildOptionsId` bigint unsigned NOT NULL,
  PRIMARY KEY (`Id`),
  KEY `IX_WithoutSupervisionChannels_GuildOptionsId` (`GuildOptionsId`),
  CONSTRAINT `FK_WithoutSupervisionChannels_Guilds_GuildOptionsId` FOREIGN KEY (`GuildOptionsId`) REFERENCES `guilds` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;
/*!40103 SET TIME_ZONE=@OLD_TIME_ZONE */;

/*!40101 SET SQL_MODE=@OLD_SQL_MODE */;
/*!40014 SET FOREIGN_KEY_CHECKS=@OLD_FOREIGN_KEY_CHECKS */;
/*!40014 SET UNIQUE_CHECKS=@OLD_UNIQUE_CHECKS */;
/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
/*!40111 SET SQL_NOTES=@OLD_SQL_NOTES */;

-- Dump completed on 2021-12-09 17:21:44
