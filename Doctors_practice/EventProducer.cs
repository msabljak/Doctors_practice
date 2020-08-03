using EventStore.ClientAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Doctors_practice
{
    public class EventProducer
    {
        static void Main(string[] args)
        {
            const string STREAM = "a_test_stream";
            const int DEFAULTPORT = 1113;
            //uncomment to enable verbose logging in client.
            var settings = ConnectionSettings.Create().DisableTls();//.EnableVerboseLogging().UseConsoleLogger();
            using (var conn = EventStoreConnection.Create(settings, new Uri("tcp://admin:changeit@localhost:1113")))
            {
                conn.ConnectAsync().Wait();
                for (var x = 0; x < 100; x++)
                {
                    conn.AppendToStreamAsync(STREAM,
                        ExpectedVersion.Any,
                        GetEventDataFor(x)).Wait();
                    //Console.WriteLine("event " + x + " written.");
                }
            }
        }

        private static EventData GetEventDataFor(int i)
        {
            return new EventData(
                Guid.NewGuid(),
                "eventType",
                true,
                Encoding.ASCII.GetBytes("{'somedata' : " + i + "}"),
                Encoding.ASCII.GetBytes("{'metadata' : " + i + "}")
                );
        }
    }
}
