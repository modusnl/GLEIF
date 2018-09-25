using System;
using System.Linq;
using System.Xml.Serialization;
using GLEIF.lei;

namespace GLEIF.ConsoleApp
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("GLEIF ConsoleApp | DEBUG");
            Console.WriteLine("Designed to run from code and control file & functions from Program.cs");
            Console.WriteLine("No CommandLine parameters implemented.`n");
            Console.ForegroundColor = ConsoleColor.White;

            // initialize stopwatch for logging / comparring processing time
            var stopwatch = new System.Diagnostics.Stopwatch();
            stopwatch.Start();

            // extract 2.6gb XML file from zip
            // place file in subfolder .\Data, DON'T check-in data in source-control!
            string fileName = @"..\..\..\..\Data\20180901-gleif-concatenated-file-lei2.xml";
            Console.WriteLine("Using file '{0}'", fileName);

            // 1.
            // Write Subsets based on serialized Full Xml 
            // --> only needs to run once, comment-out after initial run!
            // Going forward, use these Use TopN subsets for quick & limited feedback on the various processing approaches
            WriteSubsets(fileName);

            // 2.
            // Parse input file and write all possible XPath references to output, 
            // handy for defining XPath queries later on
            //XPathExtension.WriteXPathFile(fileName.Replace(".xml", "-Top100.xml"));

            // 3.
            // Validate using XSD schemas from filesystem
            //Streaming.ValidateXmlStream(fileName.Replace(".xml", "-Top10000.xml"));
            //Streaming.ValidateXmlStream(fileName);

            // 4.
            // Stream XML to XmlLines
            //Streaming.StreamXmlLines(fileName.Replace(".xml", ".xml"), "lei:LEIHeader", 1);
            //Streaming.StreamXmlLines(fileName.Replace(".xml", ".xml"), "lei:LEIRecord"); // -> 15 mb RAM, regardless of filesize! whole file = ... min

            // 5. ToDo
            // Streaming option 1: Write LEIRecord to Console via Serialization
            //Serialization.DeserializeLEIRecord(xmlString);

            // 6. ToDo
            // Streaming option 2: Write LEIRecord to Console via XPath
            //Streaming.ConsoleWriteXPathLEIRecord

            // Closing
            Console.WriteLine("Finished in {0}", stopwatch.Elapsed);
            Console.WriteLine("Press Key to exit...");
            Console.ReadKey();
        }

        private static void WriteSubsets(string fileName)
        {
            // local variable leiData as LEIData Object (classes generated from XSD using XSD.exe)
            LEIData _leiData;

            // Read Xml into LEIData object
            _leiData = Serialization.ReadXml(fileName); // 2.6gb RAM for 2.6gb XML file

            Console.WriteLine("Writing subsets...");

            // Write Top records  (using LINQ filtering)
            WriteXmlTopN(_leiData, fileName.Replace(".xml", "-Top10.xml"), 10);
            WriteXmlTopN(_leiData, fileName.Replace(".xml", "-Top100.xml"), 100);
            WriteXmlTopN(_leiData, fileName.Replace(".xml", "-Top1000.xml"), 1000);
            WriteXmlTopN(_leiData, fileName.Replace(".xml", "-Top10000.xml"), 10000);
            WriteXmlTopN(_leiData, fileName.Replace(".xml", "-Top100000.xml"), 100000);
            
            // Write Country (using LINQ filtering)
            WriteXmlCountry(_leiData, fileName.Replace(".xml", "-NL.xml"), "NL");
            WriteXmlCountry(_leiData, fileName.Replace(".xml", "-PT.xml"), "PT");

            // "-copy.xml" is only used for like4like comparision with raw downloaded file; 
            //2.6gb XML input results in 2.6gb -copy.xml
            //Serialization.WriteXml(_leiData, fileName.Replace(".xml", "-copy.xml")); 

            // Write other formats
            Serialization.WriteCsv(_leiData, fileName.Replace(".xml", ".csv"));
            Serialization.WriteJson(_leiData, fileName.Replace(".xml", ".json"));

            // Read produced JSON back into LEIData object
            // --> comment-out as it's not actively used. 
            //     retained as handy snippet for debugging though.
            //_leiData = Serialization.ReadJson(fileName.Replace(".xml", ".json"));
        }

        // filter LEIData object using LINQ expression to TopN records
        private static void WriteXmlTopN(LEIData leiData, string fileName, int count)
        {
            // Inititialize TopN object based on parent object
            LEIData _leiData = new LEIData
            {
                LEIHeader = leiData.LEIHeader,
                LEIRecords = new LEIRecordsType()
            };

            // Filter top N records, using List<> methods
            _leiData.LEIRecords.LEIRecord =
                leiData.LEIRecords.LEIRecord.Take<LEIRecordType>(count).ToList();

            // Update actual RecordCount in Header
            _leiData.LEIHeader.RecordCount = _leiData.LEIRecords.LEIRecord.Count.ToString();

            Serialization.WriteXml(_leiData, fileName);
        }

        // filter LEIData object using LINQ expression to country specific set
        // append CountryCode to NameSpace (as experiment for different formats)
        private static void WriteXmlCountry(LEIData leiData, string fileName, string countryCode)
        {
            // Inititialize TopN object based on parent object
            LEIData _leiData = new LEIData
            {
                LEIHeader = leiData.LEIHeader,
                LEIRecords = new LEIRecordsType()
            };

            // Filter only specified Country records, using LINQ Lambda Expressions
            // The Any operator enumerates the source sequence and returns true if any Country == countryCode
            _leiData.LEIRecords.LEIRecord =
                leiData.LEIRecords.LEIRecord.
                Where(rec => rec.Entity.LegalAddress.Country == countryCode ||
                             rec.Entity.HeadquartersAddress.Country == countryCode
                ).ToList();

            // Update actual RecordCount in Header
            _leiData.LEIHeader.RecordCount = _leiData.LEIRecords.LEIRecord.Count.ToString();

            // as an Experiment, use different Namespace for this country; append the CountryCode
            XmlSerializerNamespaces ns = new XmlSerializerNamespaces();
            ns.Add("gleif" + countryCode, "http://www.gleif.org/concatenated-file/header-extension/2.0");
            ns.Add("lei" + countryCode, "http://www.gleif.org/data/schema/leidata/2016");

            // Write to file
            Serialization.WriteXml(_leiData, fileName, ns);
        }
    }
}