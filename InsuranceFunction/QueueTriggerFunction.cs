using System;
using Azure.Messaging.ServiceBus;
using System.Xml.Serialization;
using Azure.Storage.Queues.Models;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Microsoft.Data.SqlClient;
using InsuranceApi.Request;
using System.Text.Json;

namespace InsuranceFunction
{
    public static class QueueTriggerFunction
    {
        private const string connectionString = "Endpoint=sb://demopluxee.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=DbSvtX2reQhyt8b/ulD8r+yZjGhEMI1UT+ASbGN801k=";
        private const string queueName = "queue1";
        private const string sqlConnectionString = "Data Source=190.94.251.11;Initial Catalog=orion_dipacar;User ID=sa;Password=Seguro1;"; 

        [FunctionName("QueueTriggerFunction")]
        public static async Task Run([ServiceBusTrigger(queueName, Connection = "AzureWebJobsServiceBus")] ServiceBusReceivedMessage message, ServiceBusClient client, ILogger log)
        {
            string body = message.Body.ToString() ;
            var insuranceRequest = JsonSerializer.Deserialize<InsuranceRequest>(body);

            if (insuranceRequest.AuthorName == "fredy") 
            {
                try
                {
                    string xmlData = ConvertToXml(insuranceRequest);

                    using (SqlConnection sqlConnection = new SqlConnection(sqlConnectionString))
                    {
                        sqlConnection.Open();
                        string query = "INSERT INTO InsuranceRequests (Data) VALUES (@Data)";
                        using (SqlCommand sqlCommand = new SqlCommand(query, sqlConnection))
                        {
                            sqlCommand.Parameters.AddWithValue("@Data", xmlData);
                            await sqlCommand.ExecuteNonQueryAsync();
                        }
                    }

                    ServiceBusReceiver receiver = client.CreateReceiver(queueName);
                    await receiver.CompleteMessageAsync(message);
                }
                catch (Exception ex)
                {
                    log.LogError($"Error storing message in DB: {ex.Message}");
                    File.AppendAllText("error.log", $"{DateTime.UtcNow}: {ex.Message}{Environment.NewLine}");
                }
            }
        }

        private static string ConvertToXml(InsuranceRequest request)
        {
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(InsuranceRequest));
            using (StringWriter textWriter = new StringWriter())
            {
                xmlSerializer.Serialize(textWriter, request);
                return textWriter.ToString();
            }
        }
    }
}
