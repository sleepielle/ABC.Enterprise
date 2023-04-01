using Proyecto.Gateway.DTOS;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Proyecto.Gateway.Services
{
    public class Transaction
    {
        private readonly IConnection _connection;
        private readonly IModel _channel;
        private readonly EventingBasicConsumer _consumer;
        private readonly HttpClient _httpClient;

        public Transaction()
        {
            var factory = new ConnectionFactory
            {
                HostName = "localhost",
                Port = 5672
            };
            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();
            _channel.QueueDeclare("gatewayQueue", false, false, false, null);


            _consumer = new EventingBasicConsumer(_channel);

        }




        public async Task<CreateTransactionDataTransferObject> ValidateTransaction(CreateTransactionDataTransferObject basketToCreate, CancellationToken token)
        {
            var error = new List<string>();
            var _info = new CreateTransactionDataTransferObject();
            if (_info.Id == _info.Id)
            {
                error.Add("La transaccion debe tener un Id valido");
            }

            await Task.Delay(2000, token);
            //_info.Status = error.Any() ? TransactionStatus.Aborted : TransactionStatus.Committed;
            //_info.Errors = error;
            return _info;

        }


    }




}

/*
 
     //public async Task<CreateTransactionDataTransferObject> ProcessTransaction(CreateTransactionDataTransferObject basketToCreate)
        //{
        //    // var result = await _httpClient.PostAsJsonAsync("/transaction/", basketToCreate);
        //    //string baseUrl = $"https://localhost:7058/gateway";
        //    //var result = await this._httpClient.PostAsJsonAsync($"{baseUrl}/transaction", basketToCreate);
        //    //result.EnsureSuccessStatusCode();
        //    //var response = await result.Content.ReadAsStringAsync();
        //    //return JsonConvert.DeserializeObject<CreateTransactionDataTransferObject>(response);

        //}
 
 */