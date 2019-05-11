using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.Storage.Blob;
using System.Net;
using System.IO;

namespace GLEIF.FunctionApp
{
    public static class DownloadZip
    {
        [FunctionName("DownloadZip")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)] HttpRequest req,
            [Blob("gleif-zip/{rand-guid}", FileAccess.Write, Connection = "GleifBlobStorage")] CloudBlockBlob randomBlob,
            ILogger logger)
        {
            // Construct Uris based on Query Parameter
            string fileDate = req.Query["fileDate"];
            Uri downloadUri = new Uri($"https://leidata.gleif.org/api/v1/concatenated-files/lei2/{fileDate}/zip");
            Uri uploadUri = new Uri($"{randomBlob.Parent.Uri.AbsoluteUri}/{fileDate}-gleif-concatenated-file-lei2.xml.zip");

            if (fileDate != null)
            {
                logger.LogInformation("Downloading '{0}' to '{1}'...", downloadUri.AbsoluteUri, uploadUri.AbsoluteUri);

                // Upload ResponseStream straight to Blob
                CloudBlockBlob outputBlob = new CloudBlockBlob(uploadUri, randomBlob.ServiceClient);
                HttpWebRequest downloadReq = (HttpWebRequest)WebRequest.Create(downloadUri);
                HttpWebResponse downloadResp = (HttpWebResponse)downloadReq.GetResponse();
                await outputBlob.UploadFromStreamAsync(downloadResp.GetResponseStream());
            }

            return fileDate != null
                ? new OkObjectResult($"Hello! \nYou've provided '{fileDate}' as fileDate. \nDowloaded '{downloadUri.AbsoluteUri}' to '{uploadUri.AbsoluteUri}'.")
                : (ActionResult)new BadRequestObjectResult("Please pass a fileDate on the query string");
        }
    }
}
