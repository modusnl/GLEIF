using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Schema;

namespace GLEIF.ConsoleApp
{
    public static class Streaming
    {
        public static void ValidateXmlStream(string fileName)
        {
            // Load validation schemas from filesystem
            XmlReaderSettings readerSettings = new XmlReaderSettings();
            readerSettings.Schemas.Add("http://www.gleif.org/concatenated-file/header-extension/2.0", @".\xsd\header-extension.2.0.xsd");
            readerSettings.Schemas.Add("http://www.gleif.org/data/schema/leidata/2016", @".\xsd\2017-03-21_lei-cdf-v2-1.xsd");
            readerSettings.Schemas.Add("http://www.w3.org/XML/1998/namespace", @".\xsd\w3.xml.1998.xsd");

            // Set validation Flags & EventHandler
            Validator validator = new Validator();
            readerSettings.ValidationType = ValidationType.Schema;
            readerSettings.ValidationFlags |= XmlSchemaValidationFlags.ReportValidationWarnings;
            readerSettings.ValidationEventHandler += new ValidationEventHandler(validator.XmlReaderValidationEventHandler);

            // Read to End
            using (XmlReader reader = XmlReader.Create(fileName, readerSettings))
            {
                while (reader.Read()) ;
                validator.WriteValidationExceptions(fileName.Replace(".xml", "-Error.txt"));
            }
        }       


        public static void WriteXmlLines(string fileName, string elementName, int elementLimit = -1)
        {
            // determine Output File Name based on provide ElementName (after the namespace) 
            string outFileName = 
                elementName.Contains(":") ?
                    fileName.Replace(".xml", "." + (elementName.Split(":")[1] + ".xml")) : 
                    fileName.Replace(".xml", "." + elementName + ".xml");

            // use StreamReader & StreamWriter to keep memory usage to a minimum
            using (XmlReader reader = XmlReader.Create(fileName))
            using (StreamWriter writer = File.CreateText(outFileName))
            {
                int elementIndex = 0;

                // define / check starting Element
                reader.MoveToContent();

                // forward reader to next available Body Element
                while (reader.ReadToFollowing(elementName))
                {
                    // Write LEIRecord as XmlLine to file
                    // Build element String
                    // Replace CRLF character (\r\n) in the XML to ensure the string fits in 1 row
                    writer.WriteLine(
                        XElement.Parse(reader.ReadOuterXml()).
                        ToString(SaveOptions.DisableFormatting).
                        Replace("\r\n", "CRLF").Replace("\n", "CRLF").Replace("\r", "CRLF")
                    );

                    // increment Index counter if ElementLimt is specified
                    if (elementLimit != -1)
                    {
                        elementIndex++;
                        if (elementIndex >= elementLimit)
                            // break wile loop if elementLimit is reached
                            break;
                    }
                }
            }
        }


        private static void ConsoleWriteXPathLEIRecord(string input)
        {
            // Load input string as XmlDoc to build (small) XML Document Object Model (DOM)
            XmlDocument xmlDocument = new XmlDocument();
            xmlDocument.LoadXml(input);
            XmlNamespaceManager nsmanager = new XmlNamespaceManager(xmlDocument.NameTable);

            // explicitly include NameSpaces, required when using XPath expression
            nsmanager.AddNamespace("gleif", "http://www.gleif.org/concatenated-file/header-extension/2.0");
            nsmanager.AddNamespace("lei", "http://www.gleif.org/data/schema/leidata/2016");
            nsmanager.AddNamespace("xml", "http://www.w3.org/XML/1998/namespace");

            // Define XPath mapping for LEIRecord
            Dictionary<string, string> mappingXPath = new Dictionary<string, string>() {
                {"//lei:Entity/lei:AssociatedEntity/@type","Entity_AssociatedEntity_type"},
                {"//lei:Entity/lei:AssociatedEntity/lei:AssociatedLEI","Entity_AssociatedEntity_AssociatedLei"},
                {"//lei:Entity/lei:EntityExpirationDate","Entity_EntityExpirationDate"},
                {"//lei:Entity/lei:EntityExpirationReason","Entity_EntityExpirationReason"},
                {"//lei:Entity/lei:EntityStatus","Entity_EntityStatus"},
                {"//lei:Entity/lei:HeadquartersAddress/@xml:lang","Entity_HeadquartersAddress_lang"},
                {"//lei:Entity/lei:HeadquartersAddress/lei:AdditionalAddressLine","Entity_HeadquartersAddress_AdditionalAddressLine"},
                {"//lei:Entity/lei:HeadquartersAddress/lei:City","Entity_HeadquartersAddress_City"},
                {"//lei:Entity/lei:HeadquartersAddress/lei:Country","Entity_HeadquartersAddress_Country"},
                {"//lei:Entity/lei:HeadquartersAddress/lei:FirstAddressLine","Entity_HeadquartersAddress_FirstAddressLine"},
                {"//lei:Entity/lei:HeadquartersAddress/lei:PostalCode","Entity_HeadquartersAddress_PostalCode"},
                {"//lei:Entity/lei:HeadquartersAddress/lei:Region","Entity_HeadquartersAddress_Region"},
                {"//lei:Entity/lei:LegalAddress/@xml:lang","Entity_LegalAddress_lang"},
                {"//lei:Entity/lei:LegalAddress/lei:AdditionalAddressLine","Entity_LegalAddress_AdditionalAddressLine"},
                {"//lei:Entity/lei:LegalAddress/lei:City","Entity_LegalAddress_City"},
                {"//lei:Entity/lei:LegalAddress/lei:Country","Entity_LegalAddress_Country"},
                {"//lei:Entity/lei:LegalAddress/lei:FirstAddressLine","Entity_LegalAddress_FirstAddressLine"},
                {"//lei:Entity/lei:LegalAddress/lei:PostalCode","Entity_LegalAddress_PostalCode"},
                {"//lei:Entity/lei:LegalAddress/lei:Region","Entity_LegalAddress_Region"},
                {"//lei:Entity/lei:LegalForm/lei:EntityLegalFormCode","Entity_LegalForm_EntityLegalFormCode"},
                {"//lei:Entity/lei:LegalForm/lei:OtherLegalForm","Entity_LegalForm_OtherLegalForm"},
                {"//lei:Entity/lei:LegalJurisdiction","Entity_LegalJurisdiction"},
                {"//lei:Entity/lei:LegalName","Entity_LegalName"},
                {"//lei:Entity/lei:LegalName/@xml:lang","Entity_LegalName_lang"},
                {"//lei:Entity/lei:NextVersion","Entity_NextVersion"},
                {"//lei:Entity/lei:OtherAddresses/lei:OtherAddress/@xml:lang","Entity_OtherAddresses_OtherAddress_lang"},
                {"//lei:Entity/lei:OtherAddresses/lei:OtherAddress/@type","Entity_OtherAddresses_OtherAddress_type"},
                {"//lei:Entity/lei:OtherAddresses/lei:OtherAddress/lei:AdditionalAddressLine","Entity_OtherAddresses_OtherAddress_AdditionalAddressLine"},
                {"//lei:Entity/lei:OtherAddresses/lei:OtherAddress/lei:City","Entity_OtherAddresses_OtherAddress_City"},
                {"//lei:Entity/lei:OtherAddresses/lei:OtherAddress/lei:Country","Entity_OtherAddresses_OtherAddress_Country"},
                {"//lei:Entity/lei:OtherAddresses/lei:OtherAddress/lei:FirstAddressLine","Entity_OtherAddresses_OtherAddress_FirstAddressLine"},
                {"//lei:Entity/lei:OtherAddresses/lei:OtherAddress/lei:PostalCode","Entity_OtherAddresses_OtherAddress_PostalCode"},
                {"//lei:Entity/lei:OtherAddresses/lei:OtherAddress/lei:Region","Entity_OtherAddresses_OtherAddress_Region"},
                {"//lei:Entity/lei:OtherEntityNames/lei:OtherEntityName","Entity_OtherEntityNames_OtherEntityName"},
                {"//lei:Entity/lei:OtherEntityNames/lei:OtherEntityName/@xml:lang","Entity_OtherEntityNames_OtherEntityName_lang"},
                {"//lei:Entity/lei:RegistrationAuthority/lei:OtherRegistrationAuthorityID","Entity_RegistrationAuthority_OtherRegistrationAuthorityID"},
                {"//lei:Entity/lei:RegistrationAuthority/lei:RegistrationAuthorityEntityID","Entity_RegistrationAuthority_RegistrationAuthorityEntityID"},
                {"//lei:Entity/lei:RegistrationAuthority/lei:RegistrationAuthorityID","Entity_RegistrationAuthority_RegistrationAuthorityID"},
                {"//lei:Entity/lei:SuccessorEntity/lei:SuccessorEntityName","Entity_SuccessorEntity_SuccessorEntityName"},
                {"//lei:Entity/lei:SuccessorEntity/lei:SuccessorLEI","Entity_SuccessorEntity_SuccessorLei"},
                {"//lei:Extension","Extension"},
                {"//lei:LEI","Lei"},
                {"//lei:NextVersion","NextVersion"},
                {"//lei:Registration/lei:InitialRegistrationDate","Registration_InitialRegistrationDate"},
                {"//lei:Registration/lei:LastUpdateDate","Registration_LastUpdateDate"},
                {"//lei:Registration/lei:ManagingLOU","Registration_ManagingLOU"},
                {"//lei:Registration/lei:NextRenewalDate","Registration_NextRenewalDate"},
                {"//lei:Registration/lei:NextVersion","Registration_NextVersion"},
                {"//lei:Registration/lei:RegistrationStatus","Registration_RegistrationStatus"},
                {"//lei:Registration/lei:ValidationAuthority/lei:OtherValidationAuthorityID","Registration_ValidationAuthority_OtherValidationAuthorityID"},
                {"//lei:Registration/lei:ValidationAuthority/lei:ValidationAuthorityEntityID","Registration_ValidationAuthority_ValidationAuthorityEntityID"},
                {"//lei:Registration/lei:ValidationAuthority/lei:ValidationAuthorityID","Registration_ValidationAuthority_ValidationAuthorityID"},
                {"//lei:Registration/lei:ValidationSources","Registration_ValidationSources"}
            };

            foreach (XmlNode xmlNode in xmlDocument.DocumentElement.SelectNodes("/lei:LEIRecord", nsmanager))
            {
                foreach (KeyValuePair<string, string> mapping in mappingXPath)
                {
                    Console.WriteLine("{0}: {1}", mapping.Value, xmlNode.SelectSingleNode(mapping.Key, nsmanager)?.InnerXml);
                }
            }
        }

                       
        public static void ReadXmlLines(string fileName)
        {
            string XmlLine;

            using (StreamReader reader = File.OpenText(fileName))
            {
                while ((XmlLine = reader.ReadLine()) != null)
                {
                    ConsoleWriteXPathLEIRecord(XmlLine);
                }
            }
        }
    }
}
