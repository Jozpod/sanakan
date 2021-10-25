CREATE TABLE `answers` (
  `Id` bigint unsigned NOT NULL AUTO_INCREMENT,
  `Number` int NOT NULL,
  `Content` varchar(50) NOT NULL,
  `QuestionId` bigint unsigned NOT NULL,
  PRIMARY KEY (`Id`),
  KEY `IX_Answers_QuestionId` (`QuestionId`),
  CONSTRAINT `FK_Answers_Questions_QuestionId` FOREIGN KEY (`QuestionId`) REFERENCES `questions` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci