using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Doctors_practice
{
    public class EventStore : IEventStore
    {
        public EventConsumer CreateEventConsumer(string stream, string group)
        {
            EventConsumer eventConsumer = new EventConsumer(stream, group);
            return eventConsumer;
        }

        public EventProducer CreateEventProducer()
        {
            EventProducer eventProducer = new EventProducer();
            return eventProducer;
        }
    }
}
