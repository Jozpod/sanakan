CREATE TABLE `questions` (
  `Id` bigint unsigned NOT NULL AUTO_INCREMENT,
  `Group` int NOT NULL,
  `AnswerNumber` int NOT NULL,
  `PointsWin` int NOT NULL,
  `PointsLose` int NOT NULL,
  `Content` varchar(100) NOT NULL,
  `TimeToAnswer` time(6) NOT NULL,
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci
