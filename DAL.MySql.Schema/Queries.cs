﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sanakan.DAL.MySql.Schema
{
    public static class Queries
	{
		public const string TableDefinition = "SHOW CREATE TABLE {0}";
		public const string TablesInDatabase = @"SELECT
	table_name
FROM INFORMATION_SCHEMA.TABLES
WHERE TABLE_SCHEMA = DATABASE()";
		public const string IndexesForTable = @"SELECT
	CONCAT('ALTER TABLE ' , TABLE_NAME, ' ', 'ADD ',
	IF(NON_UNIQUE = 1,
	CASE UPPER(INDEX_TYPE)
		WHEN 'FULLTEXT' THEN 'FULLTEXT INDEX'
		WHEN 'SPATIAL' THEN 'SPATIAL INDEX'
		ELSE CONCAT('INDEX ', INDEX_NAME, ' USING ', INDEX_TYPE)
	END,
	IF(UPPER(INDEX_NAME) = 'PRIMARY', CONCAT('PRIMARY KEY USING ', INDEX_TYPE),
	CONCAT('UNIQUE INDEX ', INDEX_NAME, ' USING ', INDEX_TYPE))
	),
	'(',
	GROUP_CONCAT(DISTINCT CONCAT('', COLUMN_NAME, '')
		ORDER BY SEQ_IN_INDEX ASC
		SEPARATOR ', '),
	');'
	) AS 'Show_Add_Indexes'
FROM information_schema.STATISTICS
WHERE TABLE_NAME = '{0}'
	AND INDEX_NAME != 'PRIMARY'
GROUP BY TABLE_NAME, INDEX_NAME
ORDER BY TABLE_NAME ASC, INDEX_NAME ASC;";
	}
}
