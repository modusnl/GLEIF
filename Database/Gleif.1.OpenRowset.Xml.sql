USE Gleif
GO

/*
Input filesizes
Top100   = 0,3Mb
Top1000  = 2Mb
Top10000 = 22Mb

https://docs.microsoft.com/en-us/sql/t-sql/functions/openxml-transact-sql?view=sql-server-2017
OPENXML() --> needs to prepare whole XML Document: EXEC sp_xml_preparedocument @idoc OUTPUT, @doc;  
Therefore use OPENROWSET() instead!

OPENROWSET() LOB types;
	SINGLE_BLOB		-> reads a file as varbinary(max)
	SINGLE_CLOB		-> reads a file as varchar(max)
	SINGLE_NCLOB	-> reads a file as nvarchar(max)
*/

-- 1.
-- Open BLob as single XmlDocument
SELECT CONVERT(XML, rs.BulkColumn) AS XmlDocument 
FROM OPENROWSET(
	BULK 'X:\Repos\GLEIF\Data\20180901-gleif-concatenated-file-lei2-Top100.xml', 
	SINGLE_BLOB
) AS rs;    


-- 2.
-- Parse XmlRecords from XmlDocument
WITH XMLNAMESPACES (
	'http://www.gleif.org/data/schema/leidata/2016' as lei
),
Input AS (
	SELECT CONVERT(XML, rs.BulkColumn) AS XmlDocument 
	FROM OPENROWSET(
		BULK 'X:\Repos\GLEIF\Data\20180901-gleif-concatenated-file-lei2-Top100.xml',  -- 7 sec
		--BULK 'X:\Repos\GLEIF\Data\20180901-gleif-concatenated-file-lei2-Top1000.xml', -- 4:07 sec
		SINGLE_BLOB
	) AS rs
)
SELECT 
	T.c.query('.') AS XmlRecord
FROM Input
CROSS APPLY XmlDocument.nodes('/lei:LEIData/lei:LEIRecords/lei:LEIRecord') AS T(c); -- 4:07 sec on Top1000


-- 3.
-- Parse XmlRecords from XmlDocument into Table for further processing
DROP TABLE IF EXISTS GleifLeiRecordXml100;
WITH XMLNAMESPACES (
	'http://www.gleif.org/data/schema/leidata/2016' as lei
),
Input AS (
	SELECT CONVERT(XML, rs.BulkColumn) AS XmlDocument 
	FROM OPENROWSET(
		BULK 'X:\Repos\GLEIF\Data\20180901-gleif-concatenated-file-lei2-Top100.xml',   --     0:02
		--BULK 'X:\Repos\GLEIF\Data\20180901-gleif-concatenated-file-lei2-Top1000.xml',  --    05:21
		--BULK 'X:\Repos\GLEIF\Data\20180901-gleif-concatenated-file-lei2-Top10000.xml', -- 05:19:32
		SINGLE_BLOB
	) AS rs
)
SELECT 
	T.c.query('.') AS XmlRecord
INTO GleifLeiRecordXml100
--INTO GleifLeiRecordXml1000
--INTO GleifLeiRecordXml10000
FROM Input
CROSS APPLY XmlDocument.nodes('/lei:LEIData/lei:LEIRecords/lei:LEIRecord') AS T(c);
SELECT * FROM GleifLeiRecordXml100;