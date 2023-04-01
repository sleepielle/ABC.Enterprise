using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Proyecto.Gateway.DTOS;
using Proyecto.Gateway.Services;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
namespace Proyecto.Gateway.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class GatewayController : ControllerBase
    {
        private readonly Transaction _TransaccionService;
        private readonly HttpClient client;
        public GatewayController(Transaction transaccionService, HttpClient client)
        {
            _TransaccionService = transaccionService;
            this.client = client;
        }

        //[HttpGet]
        //public async Task<string> Get()
        //{
        //    string hola = "hola";
        //    return await Task.FromResult(hola);
        //}

        [HttpPost("transaction")]
        public async Task<IActionResult> Post([FromBody] CreateTransactionDataTransferObject lineToCreate)
        {



            var newTransaction = new CreateTransactionDataTransferObject
            {
                Id = lineToCreate.Id,
                //suma todas las lineas
                Status = lineToCreate.Status,
                Errors = lineToCreate.Errors
            };


            var json = JsonConvert.SerializeObject(newTransaction);
            //esto es generico
            var factory = new ConnectionFactory
            {
                HostName = "localhost",
                Port = 5672,

            };

            using (var connection = factory.CreateConnection())
            {

                using (var channel = connection.CreateModel())
                {
                    channel.QueueDeclare("gatewayQueue", false, false, false, null);
                    //se manda el json a la queue en bytes. 
                    var body = Encoding.UTF8.GetBytes(json);
                    //enviar el mensaje, siempre se pone string.empty para nosotros,
                    channel.BasicPublish(string.Empty, "gatewayQueue", null, body);

                }
            }
            return Ok(newTransaction);
        }


        [HttpGet]
        public async Task<CreateTransactionDataTransferObject> Get()
        {
            var factory = new ConnectionFactory() { HostName = "localhost" };
            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                channel.QueueDeclare(queue: "toGateway", durable: false, exclusive: false, autoDelete: false, arguments: null);

                var tcs = new TaskCompletionSource<CreateTransactionDataTransferObject>();
                var consumer = new EventingBasicConsumer(channel);
                consumer.Received += async (model, ea) =>
                {
                    var body = ea.Body.ToArray();
                    var message = Encoding.UTF8.GetString(body);
                    var dto = JsonConvert.DeserializeObject<CreateTransactionDataTransferObject>(message);

                    channel.BasicAck(ea.DeliveryTag, false);

                    tcs.TrySetResult(dto);
                };
                channel.BasicConsume(queue: "toGateway", autoAck: false, consumer: consumer);

                var result = await tcs.Task;
                return result;
            }
        }


        //var baseUrl = $"https://localhost:7132/validations";
        //var info = await client.GetStringAsync($"{baseUrl}");
        //var result = JsonConvert.DeserializeObject<CreateTransactionDataTransferObject>(info);

        //return Ok(result);
    }
}



