using Azure.Messaging.ServiceBus;
using InsuranceApi.Entity;
using InsuranceApi.Request;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace InsuranceApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class InsuranceController : ControllerBase
    {

        private const string connectionString = "Endpoint=sb://demopluxee.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=DbSvtX2reQhyt8b/ulD8r+yZjGhEMI1UT+ASbGN801k=";
        private const string queueName = "queue1";

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] InsuranceRequest request)
        {
            try
            {
                request.ProcessDate = DateTime.UtcNow;
                request.AuthorName = "fredy"; 

                request.InsurancePayment = new InsurancePayment
                {
                    PaymentId = 1,
                    PaymentDatetime = DateTime.UtcNow,
                    Franchise = "Standard",
                    Currency = "USD",
                    Amount = 100
                };

                string messageBody = JsonSerializer.Serialize(request);

                await SendMessageAsync(messageBody);

                return Ok(new { response = true, msg = "" });
            }
            catch (Exception)
            {
                return BadRequest(new { response = false, msg = "error in the queue" });
            }
        }

        private async Task SendMessageAsync(string messageBody)
        {
            await using (ServiceBusClient client = new ServiceBusClient(connectionString))
            {
                ServiceBusSender sender = client.CreateSender(queueName);
                ServiceBusMessage message = new ServiceBusMessage(messageBody);
                await sender.SendMessageAsync(message);
            }
        }
    }
}
