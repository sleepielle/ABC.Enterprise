using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Proyecto.Validaciones.DTOS;
using RabbitMQ.Client;
using System.Text;

namespace Proyecto.Validaciones.Controller
{
    [ApiController]
    [Route("[controller]")]
    public class ValidationsController : ControllerBase
    {


        [HttpGet]
        public IActionResult Get()
        {

            string status = "";

            if (Database.errors.Count != 0)
            {
                status = "Errored";
            }
            else if (Database.errors.Count == 0)
            {
                status = "Completed";
            }

            var newTransaction = new TransactionDataTransferObject
            {
                Id = Database.id.First(),
                Status = status,
                Errors = Database.errors

            };


            var json = JsonConvert.SerializeObject(newTransaction);

            var factory = new ConnectionFactory
            {
                HostName = "localhost",
                Port = 5672
            };
            using var conn = factory.CreateConnection();
            using var channel = conn.CreateModel();
            channel.QueueDeclare("toGateway", false, false, false, null);
            var body = Encoding.UTF8.GetBytes(json);
            channel.BasicPublish(string.Empty, "toGateway", null, body);
            return Ok(newTransaction);

        }
    }
}
