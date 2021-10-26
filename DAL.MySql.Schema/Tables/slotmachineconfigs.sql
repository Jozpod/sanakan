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
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci
ALTER TABLE slotmachineconfigs ADD UNIQUE INDEX IX_SlotMachineConfigs_UserId USING BTREE(UserId);
