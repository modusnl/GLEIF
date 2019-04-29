using System;
using System.IO;
using System.Xml.Serialization;
using GLEIF.lei;
using Newtonsoft.Json;
using CsvHelper;

namespace GLEIF.FunctionApp
{
    class Serialization
    {

        // Deserialize XML file straight into LEIData Object (stream)
        public static LEIData ReadXml(Stream input)
        {
            LEIData leiData;

            XmlSerializer serializer = new XmlSerializer(typeof(LEIData));
            leiData = (LEIData)serializer.Deserialize(input);

            return leiData;
        }


        // serialize LEIData object directly to XML stream
        public static void WriteXml(LEIData leiData, Stream output, XmlSerializerNamespaces ns = null)
        {
            if (ns == null)
            {
                // explicitly include NameSpaces, to comply with 2017-03-21_lei-cdf-v2-1.pdf | 1.6.1. XML Design Rules
                ns = new XmlSerializerNamespaces();
                ns.Add("gleif", "http://www.gleif.org/concatenated-file/header-extension/2.0");
                ns.Add("lei", "http://www.gleif.org/data/schema/leidata/2016");
            }

            XmlSerializer serializer = new XmlSerializer(typeof(LEIData));
            serializer.Serialize(output, leiData, ns);
        }


        // serialize LEIData object directly to JSON file (stream)
        public static void WriteJson(LEIData leiData, Stream output)
        {
            using (StreamWriter file = new StreamWriter(output))
            {
                JsonSerializer serializer = new JsonSerializer();
                serializer.Serialize(file, leiData);
            }
        }
               

        // serialize LEIData.LEIRecords object directly to CSV file (stream)
        public static void WriteCsv(LEIData leiData, Stream output)
        {
            using (StreamWriter file = new StreamWriter(output))
            {
                CsvWriter csv = new CsvWriter(file);
                csv.WriteRecords(leiData.LEIRecords.LEIRecord);
            }
        }
    }
}
