CREATE EXTERNAL DATA SOURCE [BlobStorageGleif]
WITH 
( 
	TYPE = BLOB_STORAGE,
 	LOCATION = 'https://modusgleif.blob.core.windows.net/gleif-xml-txt'
);

-- 1a.
-- Bulk Insert Xml-Lines into table 
DROP TABLE IF EXISTS GleifLeiRecordXml10000;
CREATE TABLE GleifLeiRecordXml10000(
	[LeiRecordXml] xml NOT NULL
);

BULK INSERT GleifLeiRecordXml10000
FROM '20180901-gleif-concatenated-file-lei2-Top10000.lei.LEIRecord.xml.txt'
WITH (
	DATA_SOURCE = 'BlobStorageGleif',
	FIRSTROW  = 1,
	ROWTERMINATOR = '\n'
);

SELECT TOP 10 * FROM GleifLeiRecordXml10000;

-- 1b.
-- Bulk Insert Xml-Lines into table 
DROP TABLE IF EXISTS GleifLeiRecordXml
CREATE TABLE GleifLeiRecordXml(
	[LeiRecordXml] xml NOT NULL
);

BULK INSERT GleifLeiRecordXml
FROM '20180901-gleif-concatenated-file-lei2.lei.LEIRecord.xml.txt' 
WITH (
	DATA_SOURCE = 'BlobStorageGleif',
	FIRSTROW  = 1,
	ROWTERMINATOR = '\n'
) --> ..:.. min
SELECT TOP 10 * FROM GleifLeiRecordXml;
