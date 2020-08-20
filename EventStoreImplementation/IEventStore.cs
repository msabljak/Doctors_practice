using System;

namespace EventStoreImplementation
{
    public interface IEventStore
    {
        EventConsumer CreateEventConsumer(string stream, string group);
        EventProducer CreateEventProducer();
    }
}
