using EventStore.ClientAPI;
using EventStore.ClientAPI.SystemData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Doctors_practice
{
    public class EventConsumer
    {
        private IEventStoreConnection _conn;
        private const string STREAM = "a_test_stream";
        private const string GROUP = "a_test_group";
        private const int DEFAULTPORT = 1113;
        private static readonly UserCredentials User = new UserCredentials("admin", "changeit");
        private EventStorePersistentSubscriptionBase _subscription;

        public void Start()
        {
            //uncommet to enable verbose logging in client.
            var settings = ConnectionSettings.Create().DisableTls(); //.EnableVerboseLogging().UseConsoleLogger();

            using (_conn = EventStoreConnection.Create(settings, new Uri("tcp://admin:changeit@localhost:1113")))
            {
                _conn.ConnectAsync().Wait();

                CreateSubscription();
                ConnectToSubscription();

                //Console.WriteLine("waiting for events. press enter to exit");
                //Console.ReadLine();
            }
        }


        private void ConnectToSubscription()
        {
            var bufferSize = 10;
            var autoAck = true;

            _subscription = _conn.ConnectToPersistentSubscription(STREAM, GROUP, EventAppeared, SubscriptionDropped,
                User, bufferSize, autoAck);
        }

        private void SubscriptionDropped(EventStorePersistentSubscriptionBase eventStorePersistentSubscriptionBase,
            SubscriptionDropReason subscriptionDropReason, Exception ex)
        {
            ConnectToSubscription();
        }

        private static void EventAppeared(EventStorePersistentSubscriptionBase eventStorePersistentSubscriptionBase,
            ResolvedEvent resolvedEvent)
        {

            var data = Encoding.ASCII.GetString(resolvedEvent.Event.Data);
            //Console.WriteLine("Received: " + resolvedEvent.Event.EventStreamId + ":" + resolvedEvent.Event.EventNumber);
            //Console.WriteLine(data);
        }

        /*
        * Normally the creating of the subscription group is not done in your general executable code. 
        * Instead it is normally done as a step during an install or as an admin task when setting 
        * things up. You should assume the subscription exists in your code.
        */

        private void CreateSubscription()
        {
            PersistentSubscriptionSettings settings = PersistentSubscriptionSettings.Create()
                .DoNotResolveLinkTos()
                .StartFromCurrent();

            try
            {
                _conn.CreatePersistentSubscriptionAsync(STREAM, GROUP, settings, User).Wait();
            }
            catch (AggregateException ex)
            {
                if (ex.InnerException.GetType() != typeof(InvalidOperationException)
                    && ex.InnerException?.Message != $"Subscription group {GROUP} on stream {STREAM} already exists")
                {
                    throw;
                }
            }
        }
    }
}

