using System;
using System.IO;
using Microsoft.Azure.WebJobs;
using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.Extensions.Logging;
using System.IO.Compression;

namespace GLEIF.FunctionApp
{
    public class GleifExtractZip
    {
        [FunctionName("Deflate-Zip")]
        public static void Run(
            [BlobTrigger("gleif-zip/{name}.zip", Connection = "GleifBlobStorage")] CloudBlockBlob inputBlob, 
            string name, 
            string blobTrigger,
            ILogger log,
            ExecutionContext context)
        {
            using (Stream inputBlobStream = inputBlob.OpenReadAsync().GetAwaiter().GetResult())
            using (ZipArchive archive = new ZipArchive(inputBlobStream, ZipArchiveMode.Read))
            {
                log.LogInformation("Extracting {0} files from '{1}'...", archive.Entries.Count, inputBlob.Uri.LocalPath);

                // Loop through ALL the files in the Zip
                foreach (ZipArchiveEntry entry in archive.Entries)
                {
                    Uri outputUri = new Uri(inputBlob.Parent.Uri.AbsoluteUri + '/' + entry.Name);
                    CloudBlockBlob outputBlockBlob = new CloudBlockBlob(outputUri, inputBlob.ServiceClient);

                    log.LogInformation("Extracting '{0}' from '{1}'...", outputUri.LocalPath, inputBlob.Uri.LocalPath);

                    outputBlockBlob.UploadFromStreamAsync(entry.Open()).Wait();
                }

                // Delete input blob as results are written to output blob(s) successfully now
                //inputBlob.DeleteAsync().Wait();

                log.LogInformation("Extracting fininished successfully!");
            }
        }
    }
}
