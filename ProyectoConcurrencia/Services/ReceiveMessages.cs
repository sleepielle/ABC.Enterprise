using Newtonsoft.Json;
using Proyecto.Validaciones.DTOS;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

namespace Proyecto.Validaciones.Services
{

    public class ReceiveMessages : BackgroundService
    {

        string queueName1 = "validationsQueue";
        private readonly IConnection conn1;
        private readonly IModel channel1;
        private readonly EventingBasicConsumer consumer1;

        private readonly IConnection conn2;
        private readonly IModel channel2;
        private readonly EventingBasicConsumer consumer2;
        private readonly HttpClient client;


        Guid fromTransaction = new Guid();
        public ReceiveMessages()
        {
            var factory = new ConnectionFactory
            {
                HostName = "localhost",
                Port = 5672
            };

            conn1 = factory.CreateConnection();
            channel1 = conn1.CreateModel();
            consumer1 = new EventingBasicConsumer(channel1);
            client = new HttpClient();

            conn2 = factory.CreateConnection();
            channel2 = conn1.CreateModel();
            consumer2 = new EventingBasicConsumer(channel2);

        }

        public override Task StartAsync(CancellationToken cancellationToken)
        {
            int count = 0;

            consumer1.Received += async (model, content) =>
            {
                count++;
                var body = content.Body.ToArray();
                var json = Encoding.UTF8.GetString(body);
                var message = JsonConvert.DeserializeObject<SalesDataTransferObject>(json);

                await employeesValidation(message.username);
                await carsValidation(message.car_id);
                vinValidation(message.vin);
                buyerValidation(message.buyer_first_name, message.buyer_last_name, message.buyer_id);


            };
            consumer2.Received += async (model, content) =>
            {
                count++;
                var body = content.Body.ToArray();
                var json = Encoding.UTF8.GetString(body);
                var message = JsonConvert.DeserializeObject<TransactionDataTransferObject>(json);
                Database.id.Add(message.Id);

            };


            channel1.BasicConsume(queueName1, true, consumer1);
            channel2.BasicConsume("recolectorQueue", true, consumer2);
            sendToGateway();
            return Task.CompletedTask;
        }

        public async Task employeesValidation(string username)
        {

            var baseUrl = $"http://localhost:5500/employees/{username}";

            var employee = await client.GetStringAsync($"{baseUrl}");
            if (!string.IsNullOrEmpty(employee))
            {
                var employeesInfo = JsonConvert.DeserializeObject<EmployeesDataTransferObject>(employee);
                if (Database.sales.Any(e => e.username == employeesInfo.username))
                {
                    Database.employees.Add(employeesInfo);

                    var baseUrlBranches = $"http://localhost:5500/branches/{employeesInfo.branch_id}";
                    var branch = await client.GetStringAsync($"{baseUrlBranches}");
                    var branchesInfo = JsonConvert.DeserializeObject<BranchesDataTransferObject>(branch);

                    if (Database.employees.Any(e => e.username == branchesInfo.username))
                    {
                        Console.WriteLine($"{branchesInfo.username} SI existe en la sucursal.");

                    }
                }

            }
            else
            {
                Console.WriteLine($"{username} no existe en la sucursal.");
                string error = $"{username} no existe en la sucursal.";
                Database.errors.Add(error);

            }
        }

        public async Task carsValidation(string carId)
        {

            var baseUrl = $"http://localhost:5500/cars/{carId}";

            var car = await client.GetStringAsync($"{baseUrl}");
            if (!string.IsNullOrEmpty(car))
            {
                var carsInfo = JsonConvert.DeserializeObject<CarsDataTransferObject>(car);
                if (Database.sales.Any(e => e.car_id == carsInfo.id))
                {
                    Database.cars.Add(carsInfo);
                    var baseUrlBranches = $"http://localhost:5500/branches/{carsInfo.branch_id}";
                    var branch = await client.GetStringAsync($"{baseUrlBranches}");
                    var branchesInfo = JsonConvert.DeserializeObject<BranchesDataTransferObject>(branch);

                    if (Database.cars.Any(e => e.id == branchesInfo.car_id))
                    {
                        Console.WriteLine($"{carId} SI existe en la sucursal.");
                    }
                }
            }
            else
            {
                Console.WriteLine($"{carId} NO existe en la sucursal.");
                string error = $"{carId} NO existe en la sucursal.";
                Database.errors.Add(error);
            }
        }


        public void vinValidation(string vin)
        {
            if (vin.Length != 17)
            {
                Console.WriteLine("Car vin {0} not valid", vin);
                string error = $"Car vin {vin} not valid";
                Database.errors.Add(error);
            }
        }

        public void buyerValidation(string firstName, string lastName, string buyerId)
        {
            if (string.IsNullOrEmpty(buyerId) && string.IsNullOrEmpty(lastName))
            {
                Console.WriteLine("Buyer {0} last name and id not valid", firstName);
                string error = $"Buyer {firstName} has an invalid lastName and buyerId";
                Database.errors.Add(error);
            }
            else if (string.IsNullOrEmpty(lastName))
            {
                Console.WriteLine("Buyer {0} last Name not valid", firstName);
                string error = $"Buyer {firstName} has an invalid lastName.";
                Database.errors.Add(error);
            }
            else if (string.IsNullOrEmpty(buyerId))
            {
                Console.WriteLine("Buyer {0} id not valid", firstName);

                string error = $"Buyer {firstName} has an invalid buyerId";
                Database.errors.Add(error);
            }
        }



        public async Task sendToGateway()
        {
            var queueName = "toGateway";

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
            channel.QueueDeclare(queueName, false, false, false, null);
            var body = Encoding.UTF8.GetBytes(json);
            channel.BasicPublish(string.Empty, queueName, null, body);


        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                Console.WriteLine("Se está ejecutando.");
                await Task.Delay(1000, stoppingToken);
            }

        }



    }
}
