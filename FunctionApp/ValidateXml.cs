using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Xml.Schema;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.Storage.Blob;

namespace GLEIF.FunctionApp
{
    public static class ValidateXml
    {
        [FunctionName("ValidateXml")]
        public static void Run(
            [BlobTrigger("gleif-xml/{name}.xml", Connection = "GleifBlobStorage")/*, Disable()*/] CloudBlockBlob inputBlob,
            [Blob("gleif-val/{name}-ValidationResult.csv", FileAccess.Write, Connection = "GleifBlobStorage")] CloudBlockBlob outputBlob,
            string name,
            string blobTrigger,
            ILogger logger,
            ExecutionContext context)
        {
            logger.LogInformation("Validating '{0}'...", blobTrigger);

            // Load validation schemas from local file system
            XmlReaderSettings readerSettings = new XmlReaderSettings();
            readerSettings.Schemas.Add("http://www.gleif.org/concatenated-file/header-extension/2.0", $@"{context.FunctionAppDirectory}\xsd\header-extension.2.0.xsd");
            readerSettings.Schemas.Add("http://www.gleif.org/data/schema/leidata/2016", $@"{context.FunctionAppDirectory}\xsd\2017-03-21_lei-cdf-v2-1.xsd");
            readerSettings.Schemas.Add("http://www.w3.org/XML/1998/namespace", $@"{context.FunctionAppDirectory}\xsd\w3.xml.1998.xsd");

            // Set validation Flags & EventHandler
            Validator validator = new Validator(inputBlob.Name);
            readerSettings.ValidationType = ValidationType.Schema;
            readerSettings.ValidationFlags |= XmlSchemaValidationFlags.ReportValidationWarnings;
            readerSettings.ValidationEventHandler += new ValidationEventHandler(validator.XmlReaderValidationEventHandler);

            // Read to End
            using (XmlReader reader = XmlReader.Create(inputBlob.OpenReadAsync().Result, readerSettings))
            {
                while (reader.Read());
            }

            // Flush final Validation List<> to Blob when there are more records than just the header
            if (validator.ValidationList.Count > 1)
            {
                using (var sw = new StreamWriter(outputBlob.OpenWriteAsync().Result))
                {
                    validator.ValidationList.ForEach(i => sw.WriteLine(i));
                }
            }
        }

        // https://docs.microsoft.com/en-us/dotnet/standard/data/xml/xml-schema-xsd-validation-with-xmlschemaset
        private class Validator
        {
            private string _fileName;
            private int _validationId = 0;
            private string _validationString;

            public Validator(string FileName)
            {
                _fileName = FileName;
            }
            public List<string> ValidationList { get; set; } = new List<string> { "FileName;Id;Severity;Message" };

            // Need to consolidate all Asynchonous EventHandler calls into List<> first
            public void XmlReaderValidationEventHandler(object sender, ValidationEventArgs e)
            {
                // Update private class variables
                _validationId++;
                _validationString = string.Format("{0};{1};{2};{3}", _fileName, _validationId.ToString(), e.Severity.ToString(), e.Message); ;
                ValidationList.Add(_validationString);
            }
        }
    }
}