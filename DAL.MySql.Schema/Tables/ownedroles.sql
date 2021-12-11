CREATE TABLE `ownedroles` (
  `RoleId` bigint unsigned NOT NULL,
  `PenaltyInfoId` bigint unsigned NOT NULL,
  PRIMARY KEY (`RoleId`,`PenaltyInfoId`),
  KEY `IX_OwnedRoles_PenaltyInfoId` (`PenaltyInfoId`),
  CONSTRAINT `FK_OwnedRoles_Penalties_PenaltyInfoId` FOREIGN KEY (`PenaltyInfoId`) REFERENCES `penalties` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci
ALTER TABLE ownedroles ADD INDEX IX_OwnedRoles_PenaltyInfoId USING BTREE(PenaltyInfoId);
