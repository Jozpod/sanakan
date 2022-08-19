CREATE TABLE `timestatuses` (
  `Id` bigint unsigned NOT NULL AUTO_INCREMENT,
  `Type` tinyint unsigned NOT NULL,
  `EndsOn` datetime(6) DEFAULT NULL,
  `IntegerValue` bigint unsigned NOT NULL,
  `BooleanValue` tinyint(1) NOT NULL,
  `GuildId` bigint unsigned DEFAULT NULL,
  `UserId` bigint unsigned NOT NULL,
  PRIMARY KEY (`Id`),
  KEY `IX_TimeStatuses_Type` (`Type`),
  KEY `IX_TimeStatuses_UserId` (`UserId`),
  CONSTRAINT `FK_TimeStatuses_Users_UserId` FOREIGN KEY (`UserId`) REFERENCES `users` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci
ALTER TABLE timestatuses ADD INDEX IX_TimeStatuses_Type USING BTREE(Type);
ALTER TABLE timestatuses ADD INDEX IX_TimeStatuses_UserId USING BTREE(UserId);
