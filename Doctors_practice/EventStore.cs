using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Doctors_practice
{
    public class EventStore : IEventStore
    {
        private IConfiguration _configuration;

        public EventStore(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public EventConsumer CreateEventConsumer(string stream, string group)
        {
            EventConsumer eventConsumer = new EventConsumer(_configuration, stream, group);
            return eventConsumer;
        }

        public EventProducer CreateEventProducer()
        {
            EventProducer eventProducer = new EventProducer(_configuration);
            return eventProducer;
        }
    }
}
