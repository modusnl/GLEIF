using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.Storage.Blob;
using SendGrid.Helpers.Mail;

namespace GLEIF.FunctionApp
{
    public static class SendEmail
    {
        [FunctionName("SendEmail")]
        public static void Run(
            [BlobTrigger("gleif-val/{name}.csv", Connection = "GleifBlobStorage")/*, Disable()*/] CloudBlockBlob inputBlob,
            [SendGrid(ApiKey = "SendGridAPIKey", To = "azure@modus.nl", From = "no-reply@modus.nl")] out SendGridMessage message,
            string name,
            string blobTrigger,
            ILogger logger,
            ExecutionContext context)
        {
            logger.LogInformation("Sending Email '{0}'...", blobTrigger);

            message = new SendGridMessage();
            message.Subject = string.Format("New Validation file '{0}'", inputBlob.Name);
            message.PlainTextContent = inputBlob.Uri.ToString();
        }
    }
}
