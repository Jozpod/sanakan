CREATE TABLE `cardtags` (
  `Id` bigint unsigned NOT NULL AUTO_INCREMENT,
  `Name` varchar(50) NOT NULL,
  `CardId` bigint unsigned NOT NULL,
  PRIMARY KEY (`Id`),
  KEY `IX_CardTags_CardId` (`CardId`),
  CONSTRAINT `FK_CardTags_Cards_CardId` FOREIGN KEY (`CardId`) REFERENCES `cards` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci