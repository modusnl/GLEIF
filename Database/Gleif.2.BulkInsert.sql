USE Gleif
GO

/*
Input filesizes
Top100   = 0,3Mb
Top1000  = 2Mb
Top10000 = 22Mb

https://docs.microsoft.com/en-us/sql/t-sql/statements/bulk-insert-transact-sql?view=sql-server-2017	
*/

-- 1a.
-- Bulk Insert Xml-Lines into table 
DROP TABLE IF EXISTS GleifLeiRecordXml10000;
CREATE TABLE GleifLeiRecordXml10000(
	[LeiRecordXml] xml NOT NULL
);

BULK INSERT GleifLeiRecordXml10000
FROM 'X:\Repos\GLEIF\Data\20180901-gleif-concatenated-file-lei2-Top10000.LEIRecord.xml' 
WITH (
	FIRSTROW  = 1,
	ROWTERMINATOR = '\n'
) --> 45 sec
SELECT TOP 10 * FROM GleifLeiRecordXml10000;


-- 1b.
-- Bulk Insert Xml-Lines into table 
DROP TABLE IF EXISTS GleifLeiRecordXml
CREATE TABLE GleifLeiRecordXml(
	[LeiRecordXml] xml NOT NULL
);

BULK INSERT GleifLeiRecordXml
FROM 'X:\Repos\GLEIF\Data\20180901-gleif-concatenated-file-lei2.LEIRecord.xml' 
WITH (
	FIRSTROW  = 1,
	ROWTERMINATOR = '\n'
) --> 12:34 min
SELECT TOP 10 * FROM GleifLeiRecordXml;