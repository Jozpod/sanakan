CREATE TABLE `timestatuses` (
  `Id` bigint unsigned NOT NULL AUTO_INCREMENT,
  `Type` int NOT NULL,
  `EndsAt` datetime NOT NULL,
  `IValue` bigint NOT NULL,
  `BValue` tinyint(1) NOT NULL,
  `Guild` bigint unsigned NOT NULL,
  `UserId` bigint unsigned NOT NULL,
  PRIMARY KEY (`Id`),
  KEY `IX_TimeStatuses_UserId` (`UserId`),
  CONSTRAINT `FK_TimeStatuses_Users_UserId` FOREIGN KEY (`UserId`) REFERENCES `users` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci