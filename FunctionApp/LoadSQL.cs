using System;
using System.IO;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.Storage.Blob;
//using Microsoft.Data.SqlClient; // -> somehow not yet supported by Azure Functions v2, therefore still stick to System.Data.SqlClient;
using System.Data.SqlClient;
using System.Data;

namespace GLEIF.FunctionApp
{
    public static class LoadSQL
    {

        [FunctionName("LoadSQL")]
        public static void Run(
            [BlobTrigger("gleif-xml-txt/{name}.xml.txt", Connection = "GleifBlobStorage")/*, Disable()*/] CloudBlockBlob inputBlob,
            string name,
            string blobTrigger,
            ILogger logger,
            ExecutionContext context)
        {
            logger.LogInformation("Loading SQL '{0}'...", blobTrigger);

            // Get the connection string from app settings and use it to create a connection.
            var connectionString = Environment.GetEnvironmentVariable("GleifSQLConnectionString");
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                // (re)Create target Table in database
                SqlCommand cmd = connection.CreateCommand();
                cmd.CommandText = $"DROP TABLE IF EXISTS [{inputBlob.Name}]";
                cmd.ExecuteNonQuery();
                cmd.CommandText = $"CREATE TABLE [{inputBlob.Name}] (XmlRecord XML)";
                cmd.ExecuteNonQuery();

                // Prepare Bulk Copy DataTable in memory
                var table = new DataTable();
                table.Columns.Add("XmlRecord", typeof(string));
                using (var reader = new StreamReader(inputBlob.OpenReadAsync().Result))
                {
                    while(!reader.EndOfStream)
                    {
                        table.Rows.Add(reader.ReadLine());
                    }
                }

                // Bulk Copy to Azure SQL
                using (var bulk = new SqlBulkCopy(connection))
                {
                    bulk.DestinationTableName = $"[{inputBlob.Name}]";
                    bulk.WriteToServer(table);
                    table.Clear();
                }
            }
        }
    }
}
