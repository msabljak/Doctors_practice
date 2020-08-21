using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EventStoreImplementation;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Apache.NMS;
using Newtonsoft.Json;
using System.Text;
using EventStore.ClientAPI;
using System.Text.RegularExpressions;

namespace EmailService
{
    public class EmailWorker : BackgroundService
    {
        private IConfiguration _configuration;
        private readonly ILogger<EmailWorker> _logger;
        private IClient _client;
        private IEventStore _eventStore;

        public EmailWorker(ILogger<EmailWorker> logger, IClient client, IConfiguration configuration, IEventStore eventStore)
        {
            _configuration = configuration;
            _logger = logger;
            _client = client;
            _eventStore = eventStore;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                var data = JsonConvert.SerializeObject(_client.ReadNextObjectMessage("EmailQueue"));
                if (data.Contains("PatientCreated"))
                {
                    string objectId = GetAttributeValue(GetAttributeValuePair(data, "ID"));
                    var dataObject = JsonConvert.DeserializeObject(data);
                    using (EventProducer eventProducer = _eventStore.CreateEventProducer())
                    {
                        try
                        {
                            _logger.LogInformation("Email confirmation sent!");
                            EventData eventData = eventProducer.CreateEventData(dataObject, "EmailConfirmation-Failed");
                            eventProducer.SendEvent($"AMQMessages-Patient-{objectId}", eventData);
                        }
                        catch (Exception)
                        {
                            _logger.LogInformation("Email confirmation sent!");
                            EventData eventData = eventProducer.CreateEventData(dataObject, "EmailConfirmation-Failed");
                            eventProducer.SendEvent($"AMQMessages-Patient-{objectId}", eventData);
                        }
                    }
                }
                await Task.Delay(1000, stoppingToken);
            }
        }
        private string GetAttributeValuePair(string objectJSON, string attributeName)
        {
            string keyValuePair = null;
            List<string> objectAttributes = objectJSON.Split(",").ToList();
            foreach (string attribute in objectAttributes)
            {
                if (attribute.Contains(attributeName))
                {
                    keyValuePair = attribute;
                }
            }
            return keyValuePair;

            //return objectJSON.Split(",").ToList().FirstOrDefault(keyValuePair => keyValuePair.Contains(attributeName));
        }

        private string GetAttributeValue(string keyValuePair)
        {

            string value = Regex.Replace(keyValuePair.Split(":")[1], @"[^\w\.@-]", "");
            return value;
        }
    }
}
