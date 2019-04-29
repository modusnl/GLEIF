using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Xml.Schema;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.Storage.Blob;

namespace GLEIF.FunctionApp
{
    public static class GleifValidateXml
    {
        [FunctionName("Validate-Xml")]
        public static void Run(
            [BlobTrigger("gleif-zip/{name}.xml", Connection = "GleifBlobStorage")] Stream inputStream,
            [Blob("gleif-xsd/header-extension.2.0.xsd", Connection = "GleifBlobStorage")] CloudBlockBlob xsdHeader,
            [Blob("gleif-xsd/2017-03-21_lei-cdf-v2-1.xsd", Connection = "GleifBlobStorage")] CloudBlockBlob xsdLEIData,
            [Blob("gleif-xsd/w3.xml.1998.xsd", Connection = "GleifBlobStorage")] CloudBlockBlob xsdNS,
            [Blob("gleif-val/{name}-ValidationResult.csv", Connection = "GleifBlobStorage")] CloudBlockBlob outputBlob,
            string name,
            string blobTrigger,
            ILogger log,
            ExecutionContext context)
        {
            log.LogInformation("Validating {0}...", blobTrigger);

            // Load validation schemas from Blob
            XmlReaderSettings readerSettings = new XmlReaderSettings();
            readerSettings.Schemas.Add("http://www.gleif.org/concatenated-file/header-extension/2.0", xsdHeader.Uri.AbsoluteUri);
            readerSettings.Schemas.Add("http://www.gleif.org/data/schema/leidata/2016", xsdLEIData.Uri.AbsoluteUri);
            readerSettings.Schemas.Add("http://www.w3.org/XML/1998/namespace", xsdNS.Uri.AbsoluteUri);

            // Set validation Flags & EventHandler
            Validator validator = new Validator();
            readerSettings.ValidationType = ValidationType.Schema;
            readerSettings.ValidationFlags |= XmlSchemaValidationFlags.ReportValidationWarnings;
            readerSettings.ValidationEventHandler += new ValidationEventHandler(validator.XmlReaderValidationEventHandler);

            // Read to End
            using (XmlReader reader = XmlReader.Create(inputStream, readerSettings))
            {
                while (reader.Read()) ;
            }

            // Flush final List<> to Blob
            File.WriteAllLines(@".\ValidationResult.csv", validator.ValidationList);
            outputBlob.UploadFromFileAsync(@".\ValidationResult.csv").Wait();
        }

        // https://docs.microsoft.com/en-us/dotnet/standard/data/xml/xml-schema-xsd-validation-with-xmlschemaset
        private class Validator
        {
            private int _validationCount = 0;
            private string _validationString;

            public List<string> ValidationList { get; set; } = new List<string>();

            // Need to consolidate all Asynchonous EventHandler calls into List<> first
            public void XmlReaderValidationEventHandler(object sender, ValidationEventArgs e)
            {
                // Update private class variables
                _validationCount++;
                _validationString = string.Format("{0};{1};{2}", e.Severity.ToString(), _validationCount.ToString(), e.Message);
                ValidationList.Add(_validationString);
            }
        }
    }
}