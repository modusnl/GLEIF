using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.Storage.Blob;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;

namespace GLEIF.FunctionApp
{
    public static class ExtractEntity
    {
        [FunctionName("ExtractEntity")]
        public static void Run(
            [BlobTrigger("gleif-xml/{name}.xml", Connection = "GleifBlobStorage")] CloudBlockBlob inputBlob,
            [Blob("gleif-xml-txt/{name}.xml.txt", Connection = "GleifBlobStorage")] CloudBlockBlob outputBlob,
            string name,
            string blobTrigger,
            ILogger logger,
            ExecutionContext context)
        {
            var elementNames = new List<string>
            {
                "lei:LEIHeader",
                "lei:LEIRecord",
                "lei:OtherAddress"
            };

            Parallel.ForEach(elementNames, (elementName) =>
            {
                logger.LogInformation("Extracting entity {0} from {1}...", elementName, blobTrigger);

                var outputUri = new Uri(outputBlob.Parent.Uri.AbsoluteUri + '/' + name + '.' + elementName.Replace(":", ".") + ".xml.txt");
                    CloudBlockBlob entityBlob = new CloudBlockBlob(outputUri, outputBlob.ServiceClient);

                    // use XML Reader & Writer for streaming the XML to keep memory usage to a minimum
                    using (var reader = XmlReader.Create(inputBlob.OpenReadAsync().Result))
                    using (var writer = new StreamWriter(entityBlob.OpenWriteAsync().Result))
                    {
                        reader.MoveToContent();

                        // forward reader to next available Element
                        while (reader.ReadToFollowing(elementName))
                        {
                            // decouple from reader position with new subtreeReader
                            // this prevents reader.ReadToFollowing() from skipping rows as its not forwarded now by ReadOuterXml()
                            using (XmlReader subtreeReader = reader.ReadSubtree())
                            {
                                subtreeReader.MoveToContent();

                                // Replace CRLF & CR & LF character (\r\n) by space ( ) within the XML to ensure the string fits in 1 row
                                writer.WriteLine(
                                    XElement.Parse(subtreeReader.ReadOuterXml()).
                                    ToString(SaveOptions.DisableFormatting).
                                    Replace("\r\n", " ").Replace('\n', ' ').Replace('\r', ' ')
                                );
                            }
                        }
                    }
                }
            );
        }
    }
}
