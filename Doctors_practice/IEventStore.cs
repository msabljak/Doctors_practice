using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Doctors_practice
{
    public interface IEventStore
    {
        EventConsumer CreateEventConsumer(string stream, string group);
        EventProducer CreateEventProducer();
    }
}
