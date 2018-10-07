using System;
using System.IO;
using System.Xml.Serialization;
using CsvHelper;
using GLEIF.lei;
using Newtonsoft.Json;

namespace GLEIF.ConsoleApp
{
    class Serialization
    {

        // Deserialize XML file straight into LEIData Object (stream)
        public static LEIData ReadXml(string fileName)
        {
            LEIData leiData;
            Console.WriteLine("Reading LEIData from XML file...");
            using (StreamReader file = File.OpenText(fileName))
            {
                XmlSerializer serializer = new XmlSerializer(typeof(LEIData));
                leiData = (LEIData)serializer.Deserialize(file);
            }
            Console.WriteLine("XML LEIHeader contains RecordCount: {0}", leiData.LEIHeader.RecordCount);
            Console.WriteLine("XML LEIRecord[0].LEI: " + leiData.LEIRecords.LEIRecord[0].LEI);
            return leiData;
        }


        // serialize LEIData object directly to XML file (stream)
        public static void WriteXml(LEIData leiData, string fileName, XmlSerializerNamespaces ns = null)
        {
            Console.WriteLine("Writing leiData to XML file '{0}'...", fileName);
            using (StreamWriter file = File.CreateText(fileName))
            {
                if (ns == null)
                {
                    // explicitly include NameSpaces, to comply with 2017-03-21_lei-cdf-v2-1.pdf | 1.6.1. XML Design Rules
                    ns = new XmlSerializerNamespaces();
                    ns.Add("gleif", "http://www.gleif.org/concatenated-file/header-extension/2.0");
                    ns.Add("lei", "http://www.gleif.org/data/schema/leidata/2016");
                }

                XmlSerializer serializer = new XmlSerializer(typeof(LEIData));
                serializer.Serialize(file, leiData, ns);
            }
        }


        // Deserialize JSON file straight into LEIData Object (stream)
        public static LEIData ReadJson(string fileName)
        {
            LEIData leiData;
            Console.WriteLine("Reading LEIData from JSON file...");
            using (StreamReader file = File.OpenText(fileName))
            {
                JsonSerializer serializer = new JsonSerializer();
                leiData = (LEIData)serializer.Deserialize(file, typeof(LEIData));
            }
            Console.WriteLine("JSON LEIHeader contains RecordCount: {0}", leiData.LEIHeader.RecordCount);
            Console.WriteLine("JSON LEIRecord[0].LEI: " + leiData.LEIRecords.LEIRecord[0].LEI);
            return leiData;
        }


        // serialize LEIData object directly to JSON file (stream)
        public static void WriteJson(LEIData leiData, string fileName)
        {
            Console.WriteLine("Writing LEIData to JSON file...");
            using (StreamWriter file = File.CreateText(fileName))
            {
                JsonSerializer serializer = new JsonSerializer();
                serializer.Serialize(file, leiData);
            }
        }
               

        // serialize LEIData.LEIRecords object directly to CSV file (stream)
        public static void WriteCsv(LEIData leiData, string fileName)
        {
            Console.WriteLine("Writing LEIData to CSV file...");
            using (StreamWriter file = File.CreateText(fileName))
            {
                CsvWriter csv = new CsvWriter(file);
                csv.WriteRecords(leiData.LEIRecords.LEIRecord);
            }
        }


        // Deserialize input string to LEIRecordType
        public static LEIRecordType DeserializeLEIRecord(string input)
        {

            using (TextReader reader = new StringReader(input))
            {
                // Deserialize XML string straight into LEIRecordType Object (stream)
                XmlSerializer serializer = new XmlSerializer(typeof(LEIRecordType));
                LEIRecordType lEIRecord = (LEIRecordType)serializer.Deserialize(reader);

                // Write to Console for debugging
                ConsoleWriteLEIRecord(lEIRecord);

                return lEIRecord;
            }
        }


        // Write LEIRecord fields to Console
        private static void ConsoleWriteLEIRecord(LEIRecordType lEIRecord)
        {
            Console.WriteLine("Entity_AssociatedEntity_type: " + (lEIRecord?.Entity?.AssociatedEntity?.type)?.ToString());
            Console.WriteLine("Entity_AssociatedEntity_AssociatedLei: " + (lEIRecord?.Entity?.AssociatedEntity?.Item)?.ToString());
            Console.WriteLine("Entity_EntityExpirationDate: " + (lEIRecord?.Entity?.EntityExpirationDate)?.ToString());
            Console.WriteLine("Entity_EntityExpirationReason: " + (lEIRecord?.Entity?.EntityExpirationReason)?.ToString());
            Console.WriteLine("Entity_EntityStatus: " + (lEIRecord?.Entity?.EntityStatus)?.ToString());
            Console.WriteLine("Entity_HeadquartersAddress_lang: " + (lEIRecord?.Entity?.HeadquartersAddress?.lang)?.ToString());
            Console.WriteLine("Entity_HeadquartersAddress_AdditionalAddressLine: " + (lEIRecord?.Entity?.HeadquartersAddress?.AdditionalAddressLine[0])?.ToString());
            Console.WriteLine("Entity_HeadquartersAddress_City: " + (lEIRecord?.Entity?.HeadquartersAddress?.City)?.ToString());
            Console.WriteLine("Entity_HeadquartersAddress_Country: " + (lEIRecord?.Entity?.HeadquartersAddress?.Country)?.ToString());
            Console.WriteLine("Entity_HeadquartersAddress_FirstAddressLine: " + (lEIRecord?.Entity?.HeadquartersAddress?.FirstAddressLine)?.ToString());
            Console.WriteLine("Entity_HeadquartersAddress_PostalCode: " + (lEIRecord?.Entity?.HeadquartersAddress?.PostalCode)?.ToString());
            Console.WriteLine("Entity_HeadquartersAddress_Region: " + (lEIRecord?.Entity?.HeadquartersAddress?.Region)?.ToString());
            Console.WriteLine("Entity_LegalAddress_lang: " + (lEIRecord?.Entity?.LegalAddress?.lang)?.ToString());
            Console.WriteLine("Entity_LegalAddress_AdditionalAddressLine: " + (lEIRecord?.Entity?.LegalAddress?.AdditionalAddressLine[0])?.ToString());
            Console.WriteLine("Entity_LegalAddress_City: " + (lEIRecord?.Entity?.LegalAddress?.City)?.ToString());
            Console.WriteLine("Entity_LegalAddress_Country: " + (lEIRecord?.Entity?.LegalAddress?.Country)?.ToString());
            Console.WriteLine("Entity_LegalAddress_FirstAddressLine: " + (lEIRecord?.Entity?.LegalAddress?.FirstAddressLine)?.ToString());
            Console.WriteLine("Entity_LegalAddress_PostalCode: " + (lEIRecord?.Entity?.LegalAddress?.PostalCode)?.ToString());
            Console.WriteLine("Entity_LegalAddress_Region: " + (lEIRecord?.Entity?.LegalAddress?.Region)?.ToString());
            Console.WriteLine("Entity_LegalForm_EntityLegalFormCode: " + (lEIRecord?.Entity?.LegalForm?.EntityLegalFormCode)?.ToString());
            Console.WriteLine("Entity_LegalForm_OtherLegalForm: " + (lEIRecord?.Entity?.LegalForm?.OtherLegalForm)?.ToString());
            Console.WriteLine("Entity_LegalJurisdiction: " + (lEIRecord?.Entity?.LegalJurisdiction)?.ToString());
            Console.WriteLine("Entity_LegalName: " + (lEIRecord?.Entity?.LegalName?.Value)?.ToString());
            Console.WriteLine("Entity_LegalName_lang: " + (lEIRecord?.Entity?.LegalName?.lang)?.ToString());
            Console.WriteLine("Entity_NextVersion: " + (lEIRecord?.Entity?.NextVersion)?.ToString());
            Console.WriteLine("Entity_OtherAddresses_OtherAddress_lang: " + (lEIRecord?.Entity?.OtherAddresses?[0]?.lang)?.ToString());
            Console.WriteLine("Entity_OtherAddresses_OtherAddress_type: " + (lEIRecord?.Entity?.OtherAddresses?[0]?.type)?.ToString());
            Console.WriteLine("Entity_OtherAddresses_OtherAddress_AdditionalAddressLine: " + (lEIRecord?.Entity?.OtherAddresses?[0]?.AdditionalAddressLine[0])?.ToString());
            Console.WriteLine("Entity_OtherAddresses_OtherAddress_City: " + (lEIRecord?.Entity?.OtherAddresses?[0]?.City)?.ToString());
            Console.WriteLine("Entity_OtherAddresses_OtherAddress_Country: " + (lEIRecord?.Entity?.OtherAddresses?[0]?.Country)?.ToString());
            Console.WriteLine("Entity_OtherAddresses_OtherAddress_FirstAddressLine: " + (lEIRecord?.Entity?.OtherAddresses?[0]?.FirstAddressLine)?.ToString());
            Console.WriteLine("Entity_OtherAddresses_OtherAddress_PostalCode: " + (lEIRecord?.Entity?.OtherAddresses?[0]?.PostalCode)?.ToString());
            Console.WriteLine("Entity_OtherAddresses_OtherAddress_Region: " + (lEIRecord?.Entity?.OtherAddresses?[0]?.Region)?.ToString());
            Console.WriteLine("Entity_OtherEntityNames_OtherEntityName: " + (lEIRecord?.Entity?.OtherEntityNames?[0].Value)?.ToString());
            Console.WriteLine("Entity_OtherEntityNames_OtherEntityName_lang: " + (lEIRecord?.Entity?.OtherEntityNames?[0]?.lang)?.ToString());
            Console.WriteLine("Entity_RegistrationAuthority_OtherRegistrationAuthorityID: " + (lEIRecord?.Entity?.RegistrationAuthority?.OtherRegistrationAuthorityID)?.ToString());
            Console.WriteLine("Entity_RegistrationAuthority_RegistrationAuthorityEntityID: " + (lEIRecord?.Entity?.RegistrationAuthority?.RegistrationAuthorityEntityID)?.ToString());
            Console.WriteLine("Entity_RegistrationAuthority_RegistrationAuthorityID: " + (lEIRecord?.Entity?.RegistrationAuthority?.RegistrationAuthorityID)?.ToString());
            Console.WriteLine("Entity_SuccessorEntity_SuccessorEntityName: " + (lEIRecord?.Entity?.SuccessorEntity?.Item)?.ToString());
            Console.WriteLine("Entity_SuccessorEntity_SuccessorLei: " + (lEIRecord?.Entity?.SuccessorEntity?.Item)?.ToString());
            Console.WriteLine("Extension: " + (lEIRecord?.Extension)?.ToString());
            Console.WriteLine("Lei: " + (lEIRecord?.LEI)?.ToString());
            Console.WriteLine("NextVersion: " + (lEIRecord?.NextVersion)?.ToString());
            Console.WriteLine("Registration_InitialRegistrationDate: " + (lEIRecord?.Registration?.InitialRegistrationDate)?.ToString());
            Console.WriteLine("Registration_LastUpdateDate: " + (lEIRecord?.Registration?.LastUpdateDate)?.ToString());
            Console.WriteLine("Registration_ManagingLOU: " + (lEIRecord?.Registration?.ManagingLOU)?.ToString());
            Console.WriteLine("Registration_NextRenewalDate: " + (lEIRecord?.Registration?.NextRenewalDate)?.ToString());
            Console.WriteLine("Registration_NextVersion: " + (lEIRecord?.Registration?.NextVersion)?.ToString());
            Console.WriteLine("Registration_RegistrationStatus: " + (lEIRecord?.Registration?.RegistrationStatus)?.ToString());
            Console.WriteLine("Registration_ValidationAuthority_OtherValidationAuthorityID: " + (lEIRecord?.Registration?.ValidationAuthority?.OtherValidationAuthorityID)?.ToString());
            Console.WriteLine("Registration_ValidationAuthority_ValidationAuthorityEntityID: " + (lEIRecord?.Registration?.ValidationAuthority?.ValidationAuthorityEntityID)?.ToString());
            Console.WriteLine("Registration_ValidationAuthority_ValidationAuthorityID: " + (lEIRecord?.Registration?.ValidationAuthority?.ValidationAuthorityID)?.ToString());
            Console.WriteLine("Registration_ValidationSources: " + (lEIRecord?.Registration?.ValidationSources)?.ToString());
        }


        // Read file with XmlLines and Deserialize LEIRecords to Console
        public static void ReadXmlLines(string fileName)
        {
            string XmlLine;

            using (StreamReader reader = File.OpenText(fileName))
            {
                while((XmlLine = reader.ReadLine()) != null)
                {
                    ConsoleWriteLEIRecord(DeserializeLEIRecord(XmlLine));
                }
            }
        }
    }
}
