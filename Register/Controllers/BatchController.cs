using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using RabbitMQ.Client;
using Register.Database;
using Register.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Register.Controllers
{
    [ApiController]
    [Route("v1/api")]
    public class BatchController : ControllerBase
    {
        private readonly IBatchDataService _batchDataService;

        private readonly ILogger<BatchController> _logger;

        public BatchController(ILogger<BatchController> logger, IBatchDataService batchDataService)
        {
            _logger = logger;
            _batchDataService = batchDataService;
        }

        [HttpGet("Batch")]
        public IActionResult Get()
        {
            var results = _batchDataService.Get();
            if (results.Count() == 0)
                return BadRequest("No records found");
            return Ok(results);
        }

        [HttpPost("Batch")]
        public IActionResult Post([FromBody] List<BatchModel> batchModels)
        {
            try
            {
                foreach (var model in batchModels)
                {
                    _batchDataService.Update(model.BatchId, model.Status);
                }
                var connectionFactory = new ConnectionFactory
                {
                    Uri = new Uri("amqp://guest:guest@localhost:5672")
                };
                using var connection = connectionFactory.CreateConnection();
                foreach (var model in batchModels)
                {
                    using var channel = connection.CreateModel();
                    channel.QueueDeclare("Batch-Start",
                      durable: true,
                      exclusive: false,
                      autoDelete: false,
                      arguments: null);
                    ProducerQueue(channel, model);
                    if (model.BatchId == 3 || model.BatchId == 7 || model.BatchId == 8 || model.BatchId == 2)
                        Thread.Sleep(3000);
                    Thread.Sleep(2000);
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
            return Ok("Batches are sheduled, will notify via eamil.");
        }

        private void ProducerQueue(IModel channel, BatchModel model)
        {
            var body = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(model));

            channel.BasicPublish("", "Batch-Start", null, body);
        }
    }
}
