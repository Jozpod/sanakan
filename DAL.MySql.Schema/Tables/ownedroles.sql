CREATE TABLE `ownedroles` (
  `Id` bigint unsigned NOT NULL AUTO_INCREMENT,
  `Role` bigint unsigned NOT NULL,
  `PenaltyInfoId` bigint unsigned NOT NULL,
  PRIMARY KEY (`Id`),
  KEY `IX_OwnedRoles_PenaltyInfoId` (`PenaltyInfoId`),
  CONSTRAINT `FK_OwnedRoles_Penalties_PenaltyInfoId` FOREIGN KEY (`PenaltyInfoId`) REFERENCES `penalties` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci