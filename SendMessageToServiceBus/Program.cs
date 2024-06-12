using Azure.Messaging.ServiceBus;
using InsuranceApi.Entity;
using InsuranceApi.Request;
using System.Text.Json;

class Program
{
    private const string connectionString = "Endpoint=sb://demopluxee.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=DbSvtX2reQhyt8b/ulD8r+yZjGhEMI1UT+ASbGN801k=";
    private const string queueName = "queue1";

    static async Task Main(string[] args)
    {
        var client = new ServiceBusClient(connectionString);
        var sender = client.CreateSender(queueName);

        var request = new InsuranceRequest
        {
            Id = 1,
            Name = "John",
            Surname = "Doe",
            ProcessDate = DateTime.UtcNow,
            AuthorName = "fredy", 
            InsurancePayment = new InsurancePayment
            {
                PaymentId = 1,
                PaymentDatetime = DateTime.UtcNow,
                Franchise = "Standard",
                Currency = "USD",
                Amount = 100
            }
        };

        string messageBody = JsonSerializer.Serialize(request);
        var message = new ServiceBusMessage(messageBody);

        await sender.SendMessageAsync(message);

        Console.WriteLine("Message sent successfully.");
    }
}