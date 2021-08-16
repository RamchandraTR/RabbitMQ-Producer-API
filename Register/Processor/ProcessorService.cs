using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Register.Model;
using Register.Database;
using Npgsql;

namespace Register.Processor
{
    public class ProcessorService : BackgroundService
    {
        private IConnection _connection;
        private IModel _channel;
        private readonly ILogger _logger;
        private readonly IBatchDataService _batchService;
        public ProcessorService(ILoggerFactory loggerFactory)
        {
            this._logger = loggerFactory.CreateLogger<ProcessorService>();
            InitRabbitMq();
        }
        private void InitRabbitMq()
        {
            var connectionFactory = new ConnectionFactory
            {
                Uri = new Uri("amqp://guest:guest@localhost:5672")
            };
            _connection = connectionFactory.CreateConnection();
            _channel = _connection.CreateModel();
            _channel.QueueDeclare("Batch-Complete",
                durable: true,
                exclusive: false,
                autoDelete: false,
                arguments: null);
            _connection.ConnectionShutdown += RabbitMQ_ConnectionShutdown;
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            stoppingToken.ThrowIfCancellationRequested();

            var consumer = new EventingBasicConsumer(_channel);
            consumer.Received += (sender, e) =>
            {
                var body = e.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);
                SendMail(message);
                Console.WriteLine(message);
            };

            _channel.BasicConsume("Batch-Complete", true, consumer);
            return Task.CompletedTask;
        }

        private void HandleMessage(string content)
        {
            // we just print this message   
            _logger.LogInformation($"consumer received {content}");
        }

        private void OnConsumerConsumerCancelled(object sender, ConsumerEventArgs e) { }
        private void OnConsumerUnregistered(object sender, ConsumerEventArgs e) { }
        private void OnConsumerRegistered(object sender, ConsumerEventArgs e) { }
        private void OnConsumerShutdown(object sender, ShutdownEventArgs e) { }
        private void RabbitMQ_ConnectionShutdown(object sender, ShutdownEventArgs e) { }

        private void SendMail(string message)
        {
            var client = new SmtpClient("smtp.gmail.com", 587)
            {
                Credentials = new NetworkCredential("thopugondaram@gmail.com", "9440776503"),
                EnableSsl = true
            };
            var model = JsonConvert.DeserializeObject<BatchModel>(message);
            Console.WriteLine("Sent");
            try
            {
                if (model.BatchId == 3 || model.BatchId == 7 || model.BatchId == 8 || model.BatchId == 2)
                    Thread.Sleep(1500);
                    client.Send("thopugondaram@gmail.com", "thopugondaram@gmail.com", "Batch Status", $"Id: {model.BatchId}, Status: {model.Status}");
                UpdateDB(message);

            }

            catch (Exception ex)
            {
                throw ex;
            }
        }
        private void UpdateDB(string message)
        {
            var model = JsonConvert.DeserializeObject<BatchModel>(message);
            const string connectionString = "Server=localhost; Port=5432; User Id=postgres; Password=ram0978; Database=Test; Pooling=false; Timeout=300; CommandTimeout=300";
            NpgsqlConnection connection = new NpgsqlConnection(connectionString);
            connection.Open();
            int result = 0;
            var command = connection.CreateCommand();
            command.CommandText = @"update Batch_Table set status='" + model.Status + "' where id = " + model.BatchId + "";
            result = command.ExecuteNonQuery();
        }

    }
}
