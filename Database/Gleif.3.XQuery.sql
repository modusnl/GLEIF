USE Gleif
GO

/*
Input filesizes
Top100   = 0,3Mb
Top1000  = 2Mb
Top10000 = 22Mb

https://docs.microsoft.com/en-us/sql/xquery/xquery-language-reference-sql-server?view=sql-server-2017
*/

-- 1a.
-- Parse Some values from XmlRecord in Table
DROP TABLE IF EXISTS GleifLeiCode10000;
WITH XMLNAMESPACES (
	'http://www.gleif.org/data/schema/leidata/2016' as lei
)
SELECT 
	LeiRecordXml.value('/lei:LEIRecord[1]/lei:LEI[1]', 'VARCHAR(100)') AS Lei,
	LeiRecordXml.value('/lei:LEIRecord[1]/lei:Entity[1]/lei:LegalName[1]', 'NVARCHAR(100)') AS LegalName,
	LeiRecordXml.value('/lei:LEIRecord[1]/lei:Entity[1]/lei:LegalName[1]/@xml:lang[1]', 'NVARCHAR(255)') AS Entity_LegalName_lang
INTO GleifLeiCode10000
FROM GleifLeiRecordXml10000 -- 10000 in 0:01 sec
SELECT TOP 10 * FROM GleifLeiCode10000;

-- 1b.
-- Parse some values from XmlRecord in Table
DROP TABLE IF EXISTS GleifLeiCode;
WITH XMLNAMESPACES (
	'http://www.gleif.org/data/schema/leidata/2016' as lei
)
SELECT 
	LeiRecordXml.value('/lei:LEIRecord[1]/lei:LEI[1]', 'VARCHAR(100)') AS Lei,
	LeiRecordXml.value('/lei:LEIRecord[1]/lei:Entity[1]/lei:LegalName[1]', 'NVARCHAR(100)') AS LegalName,
	LeiRecordXml.value('/lei:LEIRecord[1]/lei:Entity[1]/lei:LegalName[1]/@xml:lang[1]', 'NVARCHAR(255)') AS Entity_LegalName_lang
INTO GleifLeiCode
FROM GleifLeiRecordXml -- 1,2mln in 2:13 min
SELECT TOP 10 * FROM GleifLeiCode;


-- 2a.
-- Parse ALL values from XmlRecord in Table
DROP TABLE IF EXISTS GleifLeirecord10000;
WITH XMLNAMESPACES (
	'http://www.gleif.org/data/schema/leidata/2016' as lei
)
SELECT 
	--LeiRecordXml,
	LeiRecordXml.value('/lei:LEIRecord[1]/lei:Entity[1]/lei:AssociatedEntity[1]/@type[1]', 'nvarchar(255)') AS Entity_AssociatedEntity_type,
	LeiRecordXml.value('/lei:LEIRecord[1]/lei:Entity[1]/lei:AssociatedEntity[1]/lei:AssociatedLEI[1]', 'nvarchar(255)') AS Entity_AssociatedEntity_AssociatedLei,
	LeiRecordXml.value('/lei:LEIRecord[1]/lei:Entity[1]/lei:EntityExpirationDate[1]', 'datetime2(3)') AS Entity_EntityExpirationDate,
	LeiRecordXml.value('/lei:LEIRecord[1]/lei:Entity[1]/lei:EntityExpirationReason[1]', 'nvarchar(255)') AS Entity_EntityExpirationReason,
	LeiRecordXml.value('/lei:LEIRecord[1]/lei:Entity[1]/lei:EntityStatus[1]', 'nvarchar(255)') AS Entity_EntityStatus,
	LeiRecordXml.value('/lei:LEIRecord[1]/lei:Entity[1]/lei:HeadquartersAddress[1]/@xml:lang[1]', 'nvarchar(255)') AS Entity_HeadquartersAddress_lang,
	LeiRecordXml.value('/lei:LEIRecord[1]/lei:Entity[1]/lei:HeadquartersAddress[1]/lei:AdditionalAddressLine[1]', 'nvarchar(255)') AS Entity_HeadquartersAddress_AdditionalAddressLine,
	LeiRecordXml.value('/lei:LEIRecord[1]/lei:Entity[1]/lei:HeadquartersAddress[1]/lei:City[1]', 'nvarchar(255)') AS Entity_HeadquartersAddress_City,
	LeiRecordXml.value('/lei:LEIRecord[1]/lei:Entity[1]/lei:HeadquartersAddress[1]/lei:Country[1]', 'nvarchar(255)') AS Entity_HeadquartersAddress_Country,
	LeiRecordXml.value('/lei:LEIRecord[1]/lei:Entity[1]/lei:HeadquartersAddress[1]/lei:FirstAddressLine[1]', 'nvarchar(255)') AS Entity_HeadquartersAddress_FirstAddressLine,
	LeiRecordXml.value('/lei:LEIRecord[1]/lei:Entity[1]/lei:HeadquartersAddress[1]/lei:PostalCode[1]', 'nvarchar(255)') AS Entity_HeadquartersAddress_PostalCode,
	LeiRecordXml.value('/lei:LEIRecord[1]/lei:Entity[1]/lei:HeadquartersAddress[1]/lei:Region[1]', 'nvarchar(255)') AS Entity_HeadquartersAddress_Region,
	LeiRecordXml.value('/lei:LEIRecord[1]/lei:Entity[1]/lei:LegalAddress[1]/@xml:lang[1]', 'nvarchar(255)') AS Entity_LegalAddress_lang,
	LeiRecordXml.value('/lei:LEIRecord[1]/lei:Entity[1]/lei:LegalAddress[1]/lei:AdditionalAddressLine[1]', 'nvarchar(255)') AS Entity_LegalAddress_AdditionalAddressLine,
	LeiRecordXml.value('/lei:LEIRecord[1]/lei:Entity[1]/lei:LegalAddress[1]/lei:City[1]', 'nvarchar(255)') AS Entity_LegalAddress_City,
	LeiRecordXml.value('/lei:LEIRecord[1]/lei:Entity[1]/lei:LegalAddress[1]/lei:Country[1]', 'nvarchar(255)') AS Entity_LegalAddress_Country,
	LeiRecordXml.value('/lei:LEIRecord[1]/lei:Entity[1]/lei:LegalAddress[1]/lei:FirstAddressLine[1]', 'nvarchar(255)') AS Entity_LegalAddress_FirstAddressLine,
	LeiRecordXml.value('/lei:LEIRecord[1]/lei:Entity[1]/lei:LegalAddress[1]/lei:PostalCode[1]', 'nvarchar(255)') AS Entity_LegalAddress_PostalCode,
	LeiRecordXml.value('/lei:LEIRecord[1]/lei:Entity[1]/lei:LegalAddress[1]/lei:Region[1]', 'nvarchar(255)') AS Entity_LegalAddress_Region,
	LeiRecordXml.value('/lei:LEIRecord[1]/lei:Entity[1]/lei:LegalForm[1]/lei:EntityLegalFormCode[1]', 'nvarchar(255)') AS Entity_LegalForm_EntityLegalFormCode,
	LeiRecordXml.value('/lei:LEIRecord[1]/lei:Entity[1]/lei:LegalForm[1]/lei:OtherLegalForm[1]', 'nvarchar(255)') AS Entity_LegalForm_OtherLegalForm,
	LeiRecordXml.value('/lei:LEIRecord[1]/lei:Entity[1]/lei:LegalJurisdiction[1]', 'nvarchar(255)') AS Entity_LegalJurisdiction,
	LeiRecordXml.value('/lei:LEIRecord[1]/lei:Entity[1]/lei:LegalName[1]', 'nvarchar(4000)') AS Entity_LegalName,
	LeiRecordXml.value('/lei:LEIRecord[1]/lei:Entity[1]/lei:LegalName[1]/@xml:lang[1]', 'nvarchar(255)') AS Entity_LegalName_lang,
	LeiRecordXml.value('/lei:LEIRecord[1]/lei:Entity[1]/lei:NextVersion[1]', 'nvarchar(255)') AS Entity_NextVersion,
	LeiRecordXml.value('/lei:LEIRecord[1]/lei:Entity[1]/lei:OtherAddresses[1]/lei:OtherAddress[1]/@xml:lang[1]', 'nvarchar(255)') AS Entity_OtherAddresses_OtherAddress_lang,
	LeiRecordXml.value('/lei:LEIRecord[1]/lei:Entity[1]/lei:OtherAddresses[1]/lei:OtherAddress[1]/@type[1]', 'nvarchar(255)') AS Entity_OtherAddresses_OtherAddress_type,
	LeiRecordXml.value('/lei:LEIRecord[1]/lei:Entity[1]/lei:OtherAddresses[1]/lei:OtherAddress[1]/lei:AdditionalAddressLine[1]', 'nvarchar(255)') AS Entity_OtherAddresses_OtherAddress_AdditionalAddressLine,
	LeiRecordXml.value('/lei:LEIRecord[1]/lei:Entity[1]/lei:OtherAddresses[1]/lei:OtherAddress[1]/lei:City[1]', 'nvarchar(255)') AS Entity_OtherAddresses_OtherAddress_City,
	LeiRecordXml.value('/lei:LEIRecord[1]/lei:Entity[1]/lei:OtherAddresses[1]/lei:OtherAddress[1]/lei:Country[1]', 'nvarchar(255)') AS Entity_OtherAddresses_OtherAddress_Country,
	LeiRecordXml.value('/lei:LEIRecord[1]/lei:Entity[1]/lei:OtherAddresses[1]/lei:OtherAddress[1]/lei:FirstAddressLine[1]', 'nvarchar(255)') AS Entity_OtherAddresses_OtherAddress_FirstAddressLine,
	LeiRecordXml.value('/lei:LEIRecord[1]/lei:Entity[1]/lei:OtherAddresses[1]/lei:OtherAddress[1]/lei:PostalCode[1]', 'nvarchar(255)') AS Entity_OtherAddresses_OtherAddress_PostalCode,
	LeiRecordXml.value('/lei:LEIRecord[1]/lei:Entity[1]/lei:OtherAddresses[1]/lei:OtherAddress[1]/lei:Region[1]', 'nvarchar(255)') AS Entity_OtherAddresses_OtherAddress_Region,
	LeiRecordXml.value('/lei:LEIRecord[1]/lei:Entity[1]/lei:OtherEntityNames[1]/lei:OtherEntityName[1]', 'nvarchar(4000)') AS Entity_OtherEntityNames_OtherEntityName,
	LeiRecordXml.value('/lei:LEIRecord[1]/lei:Entity[1]/lei:OtherEntityNames[1]/lei:OtherEntityName[1]/@xml:lang[1]', 'nvarchar(255)') AS Entity_OtherEntityNames_OtherEntityName_lang,
	LeiRecordXml.value('/lei:LEIRecord[1]/lei:Entity[1]/lei:RegistrationAuthority[1]/lei:OtherRegistrationAuthorityID[1]', 'nvarchar(255)') AS Entity_RegistrationAuthority_OtherRegistrationAuthorityID,
	LeiRecordXml.value('/lei:LEIRecord[1]/lei:Entity[1]/lei:RegistrationAuthority[1]/lei:RegistrationAuthorityEntityID[1]', 'nvarchar(255)') AS Entity_RegistrationAuthority_RegistrationAuthorityEntityID,
	LeiRecordXml.value('/lei:LEIRecord[1]/lei:Entity[1]/lei:RegistrationAuthority[1]/lei:RegistrationAuthorityID[1]', 'nvarchar(255)') AS Entity_RegistrationAuthority_RegistrationAuthorityID,
	LeiRecordXml.value('/lei:LEIRecord[1]/lei:Entity[1]/lei:SuccessorEntity[1]/lei:SuccessorEntityName[1]', 'nvarchar(4000)') AS Entity_SuccessorEntity_SuccessorEntityName,
	LeiRecordXml.value('/lei:LEIRecord[1]/lei:Entity[1]/lei:SuccessorEntity[1]/lei:SuccessorLEI[1]', 'nvarchar(255)') AS Entity_SuccessorEntity_SuccessorLei,
	LeiRecordXml.value('/lei:LEIRecord[1]/lei:Extension[1]', 'nvarchar(255)') AS Extension,
	LeiRecordXml.value('/lei:LEIRecord[1]/lei:LEI[1]', 'nvarchar(255)') AS Lei,
	LeiRecordXml.value('/lei:LEIRecord[1]/lei:NextVersion[1]', 'nvarchar(255)') AS NextVersion,
	LeiRecordXml.value('/lei:LEIRecord[1]/lei:Registration[1]/lei:InitialRegistrationDate[1]', 'datetime2(3)') AS Registration_InitialRegistrationDate,
	LeiRecordXml.value('/lei:LEIRecord[1]/lei:Registration[1]/lei:LastUpdateDate[1]', 'datetime2(3)') AS Registration_LastUpdateDate,
	LeiRecordXml.value('/lei:LEIRecord[1]/lei:Registration[1]/lei:ManagingLOU[1]', 'nvarchar(255)') AS Registration_ManagingLOU,
	LeiRecordXml.value('/lei:LEIRecord[1]/lei:Registration[1]/lei:NextRenewalDate[1]', 'datetime2(3)') AS Registration_NextRenewalDate,
	LeiRecordXml.value('/lei:LEIRecord[1]/lei:Registration[1]/lei:NextVersion[1]', 'nvarchar(255)') AS Registration_NextVersion,
	LeiRecordXml.value('/lei:LEIRecord[1]/lei:Registration[1]/lei:RegistrationStatus[1]', 'nvarchar(255)') AS Registration_RegistrationStatus,
	LeiRecordXml.value('/lei:LEIRecord[1]/lei:Registration[1]/lei:ValidationAuthority[1]/lei:OtherValidationAuthorityID[1]', 'nvarchar(255)') AS Registration_ValidationAuthority_OtherValidationAuthorityID,
	LeiRecordXml.value('/lei:LEIRecord[1]/lei:Registration[1]/lei:ValidationAuthority[1]/lei:ValidationAuthorityEntityID[1]', 'nvarchar(255)') AS Registration_ValidationAuthority_ValidationAuthorityEntityID,
	LeiRecordXml.value('/lei:LEIRecord[1]/lei:Registration[1]/lei:ValidationAuthority[1]/lei:ValidationAuthorityID[1]', 'nvarchar(255)') AS Registration_ValidationAuthority_ValidationAuthorityID,
	LeiRecordXml.value('/lei:LEIRecord[1]/lei:Registration[1]/lei:ValidationSources[1]', 'nvarchar(255)') AS Registration_ValidationSources
INTO GleifLeirecord10000
FROM GleifLeirecordXml10000 -- 10000 in 0:36 sec

SELECT TOP 10 * FROM GleifLeirecord10000;

-- 2b.
-- Parse ALL values from XmlRecord in Table
DROP TABLE IF EXISTS GleifLeirecord;
WITH XMLNAMESPACES (
	'http://www.gleif.org/data/schema/leidata/2016' as lei
)
SELECT
	LeiRecordXml.value('/lei:LEIRecord[1]/lei:Entity[1]/lei:AssociatedEntity[1]/@type[1]', 'nvarchar(255)') AS Entity_AssociatedEntity_type,
	LeiRecordXml.value('/lei:LEIRecord[1]/lei:Entity[1]/lei:AssociatedEntity[1]/lei:AssociatedLEI[1]', 'nvarchar(255)') AS Entity_AssociatedEntity_AssociatedLei,
	LeiRecordXml.value('/lei:LEIRecord[1]/lei:Entity[1]/lei:EntityExpirationDate[1]', 'datetime2(3)') AS Entity_EntityExpirationDate,
	LeiRecordXml.value('/lei:LEIRecord[1]/lei:Entity[1]/lei:EntityExpirationReason[1]', 'nvarchar(255)') AS Entity_EntityExpirationReason,
	LeiRecordXml.value('/lei:LEIRecord[1]/lei:Entity[1]/lei:EntityStatus[1]', 'nvarchar(255)') AS Entity_EntityStatus,
	LeiRecordXml.value('/lei:LEIRecord[1]/lei:Entity[1]/lei:HeadquartersAddress[1]/@xml:lang[1]', 'nvarchar(255)') AS Entity_HeadquartersAddress_lang,
	LeiRecordXml.value('/lei:LEIRecord[1]/lei:Entity[1]/lei:HeadquartersAddress[1]/lei:AdditionalAddressLine[1]', 'nvarchar(255)') AS Entity_HeadquartersAddress_AdditionalAddressLine,
	LeiRecordXml.value('/lei:LEIRecord[1]/lei:Entity[1]/lei:HeadquartersAddress[1]/lei:City[1]', 'nvarchar(255)') AS Entity_HeadquartersAddress_City,
	LeiRecordXml.value('/lei:LEIRecord[1]/lei:Entity[1]/lei:HeadquartersAddress[1]/lei:Country[1]', 'nvarchar(255)') AS Entity_HeadquartersAddress_Country,
	LeiRecordXml.value('/lei:LEIRecord[1]/lei:Entity[1]/lei:HeadquartersAddress[1]/lei:FirstAddressLine[1]', 'nvarchar(255)') AS Entity_HeadquartersAddress_FirstAddressLine,
	LeiRecordXml.value('/lei:LEIRecord[1]/lei:Entity[1]/lei:HeadquartersAddress[1]/lei:PostalCode[1]', 'nvarchar(255)') AS Entity_HeadquartersAddress_PostalCode,
	LeiRecordXml.value('/lei:LEIRecord[1]/lei:Entity[1]/lei:HeadquartersAddress[1]/lei:Region[1]', 'nvarchar(255)') AS Entity_HeadquartersAddress_Region,
	LeiRecordXml.value('/lei:LEIRecord[1]/lei:Entity[1]/lei:LegalAddress[1]/@xml:lang[1]', 'nvarchar(255)') AS Entity_LegalAddress_lang,
	LeiRecordXml.value('/lei:LEIRecord[1]/lei:Entity[1]/lei:LegalAddress[1]/lei:AdditionalAddressLine[1]', 'nvarchar(255)') AS Entity_LegalAddress_AdditionalAddressLine,
	LeiRecordXml.value('/lei:LEIRecord[1]/lei:Entity[1]/lei:LegalAddress[1]/lei:City[1]', 'nvarchar(255)') AS Entity_LegalAddress_City,
	LeiRecordXml.value('/lei:LEIRecord[1]/lei:Entity[1]/lei:LegalAddress[1]/lei:Country[1]', 'nvarchar(255)') AS Entity_LegalAddress_Country,
	LeiRecordXml.value('/lei:LEIRecord[1]/lei:Entity[1]/lei:LegalAddress[1]/lei:FirstAddressLine[1]', 'nvarchar(255)') AS Entity_LegalAddress_FirstAddressLine,
	LeiRecordXml.value('/lei:LEIRecord[1]/lei:Entity[1]/lei:LegalAddress[1]/lei:PostalCode[1]', 'nvarchar(255)') AS Entity_LegalAddress_PostalCode,
	LeiRecordXml.value('/lei:LEIRecord[1]/lei:Entity[1]/lei:LegalAddress[1]/lei:Region[1]', 'nvarchar(255)') AS Entity_LegalAddress_Region,
	LeiRecordXml.value('/lei:LEIRecord[1]/lei:Entity[1]/lei:LegalForm[1]/lei:EntityLegalFormCode[1]', 'nvarchar(255)') AS Entity_LegalForm_EntityLegalFormCode,
	LeiRecordXml.value('/lei:LEIRecord[1]/lei:Entity[1]/lei:LegalForm[1]/lei:OtherLegalForm[1]', 'nvarchar(255)') AS Entity_LegalForm_OtherLegalForm,
	LeiRecordXml.value('/lei:LEIRecord[1]/lei:Entity[1]/lei:LegalJurisdiction[1]', 'nvarchar(255)') AS Entity_LegalJurisdiction,
	LeiRecordXml.value('/lei:LEIRecord[1]/lei:Entity[1]/lei:LegalName[1]', 'nvarchar(4000)') AS Entity_LegalName,
	LeiRecordXml.value('/lei:LEIRecord[1]/lei:Entity[1]/lei:LegalName[1]/@xml:lang[1]', 'nvarchar(255)') AS Entity_LegalName_lang,
	LeiRecordXml.value('/lei:LEIRecord[1]/lei:Entity[1]/lei:NextVersion[1]', 'nvarchar(255)') AS Entity_NextVersion,
	LeiRecordXml.value('/lei:LEIRecord[1]/lei:Entity[1]/lei:OtherAddresses[1]/lei:OtherAddress[1]/@xml:lang[1]', 'nvarchar(255)') AS Entity_OtherAddresses_OtherAddress_lang,
	LeiRecordXml.value('/lei:LEIRecord[1]/lei:Entity[1]/lei:OtherAddresses[1]/lei:OtherAddress[1]/@type[1]', 'nvarchar(255)') AS Entity_OtherAddresses_OtherAddress_type,
	LeiRecordXml.value('/lei:LEIRecord[1]/lei:Entity[1]/lei:OtherAddresses[1]/lei:OtherAddress[1]/lei:AdditionalAddressLine[1]', 'nvarchar(255)') AS Entity_OtherAddresses_OtherAddress_AdditionalAddressLine,
	LeiRecordXml.value('/lei:LEIRecord[1]/lei:Entity[1]/lei:OtherAddresses[1]/lei:OtherAddress[1]/lei:City[1]', 'nvarchar(255)') AS Entity_OtherAddresses_OtherAddress_City,
	LeiRecordXml.value('/lei:LEIRecord[1]/lei:Entity[1]/lei:OtherAddresses[1]/lei:OtherAddress[1]/lei:Country[1]', 'nvarchar(255)') AS Entity_OtherAddresses_OtherAddress_Country,
	LeiRecordXml.value('/lei:LEIRecord[1]/lei:Entity[1]/lei:OtherAddresses[1]/lei:OtherAddress[1]/lei:FirstAddressLine[1]', 'nvarchar(255)') AS Entity_OtherAddresses_OtherAddress_FirstAddressLine,
	LeiRecordXml.value('/lei:LEIRecord[1]/lei:Entity[1]/lei:OtherAddresses[1]/lei:OtherAddress[1]/lei:PostalCode[1]', 'nvarchar(255)') AS Entity_OtherAddresses_OtherAddress_PostalCode,
	LeiRecordXml.value('/lei:LEIRecord[1]/lei:Entity[1]/lei:OtherAddresses[1]/lei:OtherAddress[1]/lei:Region[1]', 'nvarchar(255)') AS Entity_OtherAddresses_OtherAddress_Region,
	LeiRecordXml.value('/lei:LEIRecord[1]/lei:Entity[1]/lei:OtherEntityNames[1]/lei:OtherEntityName[1]', 'nvarchar(4000)') AS Entity_OtherEntityNames_OtherEntityName,
	LeiRecordXml.value('/lei:LEIRecord[1]/lei:Entity[1]/lei:OtherEntityNames[1]/lei:OtherEntityName[1]/@xml:lang[1]', 'nvarchar(255)') AS Entity_OtherEntityNames_OtherEntityName_lang,
	LeiRecordXml.value('/lei:LEIRecord[1]/lei:Entity[1]/lei:RegistrationAuthority[1]/lei:OtherRegistrationAuthorityID[1]', 'nvarchar(255)') AS Entity_RegistrationAuthority_OtherRegistrationAuthorityID,
	LeiRecordXml.value('/lei:LEIRecord[1]/lei:Entity[1]/lei:RegistrationAuthority[1]/lei:RegistrationAuthorityEntityID[1]', 'nvarchar(255)') AS Entity_RegistrationAuthority_RegistrationAuthorityEntityID,
	LeiRecordXml.value('/lei:LEIRecord[1]/lei:Entity[1]/lei:RegistrationAuthority[1]/lei:RegistrationAuthorityID[1]', 'nvarchar(255)') AS Entity_RegistrationAuthority_RegistrationAuthorityID,
	LeiRecordXml.value('/lei:LEIRecord[1]/lei:Entity[1]/lei:SuccessorEntity[1]/lei:SuccessorEntityName[1]', 'nvarchar(4000)') AS Entity_SuccessorEntity_SuccessorEntityName,
	LeiRecordXml.value('/lei:LEIRecord[1]/lei:Entity[1]/lei:SuccessorEntity[1]/lei:SuccessorLEI[1]', 'nvarchar(255)') AS Entity_SuccessorEntity_SuccessorLei,
	LeiRecordXml.value('/lei:LEIRecord[1]/lei:Extension[1]', 'nvarchar(255)') AS Extension,
	LeiRecordXml.value('/lei:LEIRecord[1]/lei:LEI[1]', 'nvarchar(255)') AS Lei,
	LeiRecordXml.value('/lei:LEIRecord[1]/lei:NextVersion[1]', 'nvarchar(255)') AS NextVersion,
	LeiRecordXml.value('/lei:LEIRecord[1]/lei:Registration[1]/lei:InitialRegistrationDate[1]', 'datetime2(3)') AS Registration_InitialRegistrationDate,
	LeiRecordXml.value('/lei:LEIRecord[1]/lei:Registration[1]/lei:LastUpdateDate[1]', 'datetime2(3)') AS Registration_LastUpdateDate,
	LeiRecordXml.value('/lei:LEIRecord[1]/lei:Registration[1]/lei:ManagingLOU[1]', 'nvarchar(255)') AS Registration_ManagingLOU,
	LeiRecordXml.value('/lei:LEIRecord[1]/lei:Registration[1]/lei:NextRenewalDate[1]', 'datetime2(3)') AS Registration_NextRenewalDate,
	LeiRecordXml.value('/lei:LEIRecord[1]/lei:Registration[1]/lei:NextVersion[1]', 'nvarchar(255)') AS Registration_NextVersion,
	LeiRecordXml.value('/lei:LEIRecord[1]/lei:Registration[1]/lei:RegistrationStatus[1]', 'nvarchar(255)') AS Registration_RegistrationStatus,
	LeiRecordXml.value('/lei:LEIRecord[1]/lei:Registration[1]/lei:ValidationAuthority[1]/lei:OtherValidationAuthorityID[1]', 'nvarchar(255)') AS Registration_ValidationAuthority_OtherValidationAuthorityID,
	LeiRecordXml.value('/lei:LEIRecord[1]/lei:Registration[1]/lei:ValidationAuthority[1]/lei:ValidationAuthorityEntityID[1]', 'nvarchar(255)') AS Registration_ValidationAuthority_ValidationAuthorityEntityID,
	LeiRecordXml.value('/lei:LEIRecord[1]/lei:Registration[1]/lei:ValidationAuthority[1]/lei:ValidationAuthorityID[1]', 'nvarchar(255)') AS Registration_ValidationAuthority_ValidationAuthorityID,
	LeiRecordXml.value('/lei:LEIRecord[1]/lei:Registration[1]/lei:ValidationSources[1]', 'nvarchar(255)') AS Registration_ValidationSources
INTO GleifLeirecord --> 1:15:53
FROM GleifLeiRecordXml

SELECT TOP 10 * FROM GleifLeirecord