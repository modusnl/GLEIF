using System;
using System.IO;
using Microsoft.Azure.WebJobs;
using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.Extensions.Logging;
using System.IO.Compression;

namespace GLEIF.FunctionApp
{
    public class DeflateZip
    {
        [FunctionName("DeflateZip")]
        public static void Run(
            [BlobTrigger("gleif-zip/{name}.zip", Connection = "GleifBlobStorage")] CloudBlockBlob inputBlob,
            [Blob("gleif-xml/{name}", Connection = "GleifBlobStorage")] CloudBlockBlob outputBlob,
            string name, 
            string blobTrigger,
            ILogger logger,
            ExecutionContext context)
        {
            // intentionally absorb complete BlobStream in memory first, 
            // so the ZipArchive doesn't have to wait on a series of blob reads
            using (Stream inputBlobStream = inputBlob.OpenReadAsync().GetAwaiter().GetResult())
            using (ZipArchive archive = new ZipArchive(inputBlobStream, ZipArchiveMode.Read))
            {
                logger.LogInformation("Extracting {0} files from '{1}'...", archive.Entries.Count, blobTrigger);

                // Loop through ALL the files in the Zip
                foreach (ZipArchiveEntry entry in archive.Entries)
                {
                    Uri outputUri = new Uri(outputBlob.Parent.Uri.AbsoluteUri + '/' + entry.Name);
                    CloudBlockBlob outputBlockBlob = new CloudBlockBlob(outputUri, outputBlob.ServiceClient);

                    logger.LogInformation("Extracting '{0}' from '{1}'...", outputUri.LocalPath, blobTrigger);

                    outputBlockBlob.UploadFromStreamAsync(entry.Open()).Wait();
                }

                // Delete input blob as results are written to output blob(s) successfully now
                //inputBlob.DeleteAsync().Wait();

                logger.LogInformation("Extracting fininished successfully!");
            }
        }
    }
}
