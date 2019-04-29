using System;
using System.IO;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using GLEIF.lei;
using System.Linq;
using System.Xml.Serialization;
using Microsoft.WindowsAzure.Storage.Blob;

namespace GLEIF.FunctionApp
{
    public class GleifExtractSubset
    {
        [FunctionName("Extract-Subset")]
        public static void Run(
            [BlobTrigger("gleif-zip/{name}.xml", Connection = "GleifBlobStorage")] Stream inputStream,
            [Blob("gleif-sub/{name}.xml", Connection = "GleifBlobStorage")] CloudBlockBlob outputBlob,
            string name, 
            string blobTrigger,
            ILogger log,
            ExecutionContext context)
        {
            log.LogInformation("Extracting subsets from {0}...", blobTrigger);

            // local variable leiData as LEIData Object (classes generated from XSD using XSD.exe)
            LEIData _leiData;

            // Read Xml into LEIData object
            _leiData = Serialization.ReadXml(inputStream);

            using (var outputStream = new MemoryStream())
            {
                Uri outputUri = new Uri(outputBlob.Parent.Uri.AbsoluteUri + '/' + name + "-Top10.xml");
                CloudBlockBlob outputBlockBlob = new CloudBlockBlob(outputUri, outputBlob.ServiceClient);
                WriteXmlTopN(_leiData, outputStream, 10);
                outputStream.Position = 0;
                outputBlockBlob.UploadFromStreamAsync(outputStream).Wait();
            }

            using (var outputStream = new MemoryStream())
            {
                Uri outputUri = new Uri(outputBlob.Parent.Uri.AbsoluteUri + '/' + name + "-Top100.xml");
                CloudBlockBlob outputBlockBlob = new CloudBlockBlob(outputUri, outputBlob.ServiceClient);
                WriteXmlTopN(_leiData, outputStream, 100);
                outputStream.Position = 0;
                outputBlockBlob.UploadFromStreamAsync(outputStream).Wait();
            }

            //// Write Top records  (using LINQ filtering)
            //WriteXmlTopN(_leiData, blobTrigger.Replace(".xml", "-Top10.xml"), 10);
            //WriteXmlTopN(_leiData, blobTrigger.Replace(".xml", "-Top100.xml"), 100);
            //WriteXmlTopN(_leiData, blobTrigger.Replace(".xml", "-Top1000.xml"), 1000);
            //WriteXmlTopN(_leiData, blobTrigger.Replace(".xml", "-Top10000.xml"), 10000);
            //WriteXmlTopN(_leiData, blobTrigger.Replace(".xml", "-Top100000.xml"), 100000);
            //
            //// Write Country (using LINQ filtering)
            //WriteXmlCountry(_leiData, blobTrigger.Replace(".xml", "-NL.xml"), "NL");
            //WriteXmlCountry(_leiData, blobTrigger.Replace(".xml", "-PT.xml"), "PT");
            //
            //// Write other formats
            //Serialization.WriteCsv(_leiData, blobTrigger.Replace(".xml", ".csv"));
            //Serialization.WriteJson(_leiData, blobTrigger.Replace(".xml", ".json"));
        }

        // filter LEIData object using LINQ expression to TopN records
        private static void WriteXmlTopN(LEIData leiData, Stream output, int count)
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

            Serialization.WriteXml(_leiData, output);
        }

        // filter LEIData object using LINQ expression to country specific set
        // append CountryCode to NameSpace (as experiment for different formats)
        private static void WriteXmlCountry(LEIData leiData, Stream output, string countryCode)
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
            Serialization.WriteXml(_leiData, output, ns);
        }
    }
}
