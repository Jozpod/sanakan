CREATE TABLE `systemdata` (
  `Id` bigint unsigned NOT NULL AUTO_INCREMENT,
  `Value` bigint NOT NULL,
  `MeasuredOn` datetime(6) NOT NULL,
  `Type` tinyint unsigned NOT NULL,
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci
