using System;
using System.IO;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.Storage.Blob;
using GLEIF.lei;
using System.Linq;
using System.Xml.Serialization;
using Newtonsoft.Json;
using CsvHelper;

namespace GLEIF.FunctionApp
{
    public class GleifExtractSubset
    {
        [FunctionName("Extract-Subset")]
        public static void Run(
            [BlobTrigger("gleif-xml/{name}lei2.xml", Connection = "GleifBlobStorage")] Stream inputStream,
            [Blob("gleif-xml/{name}lei2.xml", Connection = "GleifBlobStorage")] CloudBlockBlob inputBlob,
            [Blob("gleif-xml/{name}", Connection = "GleifBlobStorage")] CloudBlockBlob outputBlob,
            string name,
            string blobTrigger,
            ILogger logger,
            ExecutionContext context)
        {
            logger.LogInformation("Extracting subsets from {0}... for", blobTrigger);

            // local variable leiData as LEIData Object (classes generated from XSD using XSD.exe)
            LEIData _leiData;

            // Read Xml into LEIData object
            logger.LogInformation("Serializing inputStream into LEIData object...");
            _leiData = ReadXml(inputStream);

            // Write Top records  (using LINQ filtering)
            logger.LogInformation("Writing TopN.xml files to blob...");
            WriteXmlTopN(_leiData, new Uri(outputBlob.Parent.Uri.AbsoluteUri + '/' + inputBlob.Name.Replace(".xml", "-Top10.xml")), outputBlob.ServiceClient, 10);
            WriteXmlTopN(_leiData, new Uri(outputBlob.Parent.Uri.AbsoluteUri + '/' + inputBlob.Name.Replace(".xml", "-Top100.xml")), outputBlob.ServiceClient, 100);
            WriteXmlTopN(_leiData, new Uri(outputBlob.Parent.Uri.AbsoluteUri + '/' + inputBlob.Name.Replace(".xml", "-Top1000.xml")), outputBlob.ServiceClient, 1000);
            WriteXmlTopN(_leiData, new Uri(outputBlob.Parent.Uri.AbsoluteUri + '/' + inputBlob.Name.Replace(".xml", "-Top10000.xml")), outputBlob.ServiceClient, 10000);
            WriteXmlTopN(_leiData, new Uri(outputBlob.Parent.Uri.AbsoluteUri + '/' + inputBlob.Name.Replace(".xml", "-Top100000.xml")), outputBlob.ServiceClient, 100000);

            // Write Country (using LINQ filtering)
            logger.LogInformation("Writing Country.xml files to blob...");
            WriteXmlCountry(_leiData, new Uri(outputBlob.Parent.Uri.AbsoluteUri + '/' + inputBlob.Name.Replace(".xml", "-NL.xml")), outputBlob.ServiceClient, "NL");
            WriteXmlCountry(_leiData, new Uri(outputBlob.Parent.Uri.AbsoluteUri + '/' + inputBlob.Name.Replace(".xml", "-PT.xml")), outputBlob.ServiceClient, "PT");

            // Write other formats -> ToDo: fix aggressive memory consumption using MemoryStream
            //logger.LogInformation("Writing .json & .csv files to blob...");
            //WriteJson(_leiData, new Uri(outputBlob.Parent.Uri.AbsoluteUri + '/' + inputBlob.Name.Replace(".xml", ".json")), outputBlob.ServiceClient);
            //WriteCsv(_leiData, new Uri(outputBlob.Parent.Uri.AbsoluteUri + '/' + inputBlob.Name.Replace(".xml", ".csv")), outputBlob.ServiceClient);
        }


        // Deserialize XML file straight into LEIData Object (stream)
        private static LEIData ReadXml(Stream input)
        {
            LEIData leiData;

            XmlSerializer serializer = new XmlSerializer(typeof(LEIData));
            leiData = (LEIData)serializer.Deserialize(input);

            return leiData;
        }


        // serialize LEIData object straight into XML stream
        private static void WriteXmlStream(LEIData leiData, Stream output, XmlSerializerNamespaces ns = null)
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


        // filter LEIData object using LINQ expression to TopN records
        private static void WriteXmlTopN(LEIData leiData, Uri outputUri, CloudBlobClient cloudBlobClient, int count)
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

            // Write serialized object via MemoryStream to Blob
            using (var ms = new MemoryStream())
            {
                WriteXmlStream(_leiData, ms);

                ms.Position = 0;
                CloudBlockBlob outputBlob = new CloudBlockBlob(outputUri, cloudBlobClient);
                outputBlob.UploadFromStreamAsync(ms).Wait();
            }
        }


        // filter LEIData object using LINQ expression to country specific set
        // append CountryCode to NameSpace (as experiment for different formats)
        private static void WriteXmlCountry(LEIData leiData, Uri outputUri, CloudBlobClient cloudBlobClient, string countryCode)
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

            // Write serialized object via MemoryStream to Blob
            using (var ms = new MemoryStream())
            {
                WriteXmlStream(_leiData, ms, ns);

                ms.Position = 0;
                CloudBlockBlob outputBlob = new CloudBlockBlob(outputUri, cloudBlobClient);
                outputBlob.UploadFromStreamAsync(ms).Wait();
            }
        }


        // Write serialized object via MemoryStream to Blob as JSON
        private static void WriteJson(LEIData leiData, Uri outputUri, CloudBlobClient cloudBlobClient)
        {
            using (var ms = new MemoryStream())
            using (var sw = new StreamWriter(ms))
            {
                JsonSerializer serializer = new JsonSerializer();
                serializer.Serialize(sw, leiData);

                ms.Position = 0;
                CloudBlockBlob jsonBlob = new CloudBlockBlob(outputUri, cloudBlobClient);
                jsonBlob.UploadFromStreamAsync(ms).Wait();
            }
        }


        // Write serialized object via MemoryStream to Blob as CSV
        private static void WriteCsv(LEIData leiData, Uri outputUri, CloudBlobClient cloudBlobClient)
        {
            CloudBlockBlob jsonBlob = new CloudBlockBlob(outputUri, cloudBlobClient);

            using (var ms = new MemoryStream())
            using (var sw = new StreamWriter(ms))
            {
                CsvWriter csv = new CsvWriter(sw);
                csv.WriteRecords(leiData.LEIRecords.LEIRecord);

                ms.Position = 0;
                jsonBlob.UploadFromStreamAsync(ms).Wait();
            }
        }
    }
}