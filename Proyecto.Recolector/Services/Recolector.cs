using CsvHelper;
using CsvHelper.Configuration;
using Newtonsoft.Json;
using Proyecto.Recolector.DTOS;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Globalization;
using System.Text;

namespace Proyecto.Recolector.Services
{
    public class Recolector : BackgroundService
    {

        string queueName1 = "gatewayQueue";
        private readonly IConnection conn1;
        private readonly IModel channel1;
        private readonly EventingBasicConsumer consumer1;
        private readonly HttpClient client;


        public Recolector()
        {
            var factory = new ConnectionFactory
            {
                HostName = "localhost",
                Port = 5672
            };

            conn1 = factory.CreateConnection();
            channel1 = conn1.CreateModel();
            consumer1 = new EventingBasicConsumer(channel1);


        }

        public override Task StartAsync(CancellationToken cancellationToken)
        {
            int count = 0;

            consumer1.Received += async (model, content) =>
            {
                count++;
                var body = content.Body.ToArray();
                var json = Encoding.UTF8.GetString(body);
                var message = JsonConvert.DeserializeObject<TransactionDataTransferObject>(json);

                await sendTransactionInfoToValidations(message);
            };

            channel1.BasicConsume(queueName1, true, consumer1);
            return Task.CompletedTask;
        }

        public void SendSales()
        {
            string filePath = $"C:\\Users\\pggis\\source\\repos\\ProyectoConcurrencia\\sales.csv";
            using (var reader = new StreamReader(filePath))
            using (var csvReader = new CsvReader(reader, new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                MissingFieldFound = null,
                HeaderValidated = null,
                Delimiter = ","
            }))
            {
                csvReader.Read();
                csvReader.ReadHeader();
                while (csvReader.Read())
                {
                    List<SalesDataTransferObject> dataSalesList = new List<SalesDataTransferObject>();
                    SalesDataTransferObject dataSales = new SalesDataTransferObject();
                    dataSales.username = csvReader.GetField("username");
                    dataSales.car_id = csvReader.GetField("car_id");
                    dataSales.price = csvReader.GetField("price");
                    dataSales.vin = csvReader.GetField("vin");
                    dataSales.buyer_first_name = csvReader.GetField("buyer_first_name");
                    dataSales.buyer_last_name = csvReader.GetField("buyer_last_name");
                    dataSales.buyer_id = csvReader.GetField("buyer_id");
                    dataSales.branch_id = csvReader.GetField("branch_id");
                    Console.WriteLine("Username:" + dataSales.username);
                    dataSalesList.Add(dataSales);
                    SendDataSalesToValidation(dataSales);

                    if (dataSalesList.Count == 50)  //lee 50 rows de  csv file
                    {
                        Console.WriteLine("50 en la lista\n\n"); //envia  50 rows a la clase de validacion
                        dataSalesList.Clear(); //elimina los  50 rows actualmente
                    }
                }
            }
        }


        private Task sendTransactionInfoToValidations(TransactionDataTransferObject transaction)
        {
            var json = JsonConvert.SerializeObject(transaction);

            var factory = new ConnectionFactory
            {
                HostName = "localhost",
                Port = 5672
            };

            using var conn = factory.CreateConnection();
            using var channel = conn.CreateModel();

            //declarar queue
            channel.QueueDeclare("recolectorQueue", false, false, false, null);
            var body = Encoding.UTF8.GetBytes(json);
            channel.BasicPublish(string.Empty, "recolectorQueue", null, body);
            Console.WriteLine(" [x] Sent FROM RECOLECTOR {0} ", json.ToString());
            return Task.CompletedTask;
        }

        //envia  50 rows
        private Task SendDataSalesToValidation(SalesDataTransferObject dataSalesList)
        {
            var json = JsonConvert.SerializeObject(dataSalesList);

            var factory = new ConnectionFactory
            {
                HostName = "localhost",
                Port = 5672
            };

            using var conn = factory.CreateConnection();
            using var channel = conn.CreateModel();

            //declarar queue
            channel.QueueDeclare("validationsQueue", false, false, false, null);
            var body = Encoding.UTF8.GetBytes(json);
            channel.BasicPublish(string.Empty, "validationsQueue", null, body);
            Console.WriteLine(" [x] Sent {0} ", json.ToString());
            return Task.CompletedTask;
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
