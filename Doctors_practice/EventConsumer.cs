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
        private string _stream;
        private string _group;
        private static readonly UserCredentials User = new UserCredentials("admin", "changeit");
        private EventStorePersistentSubscriptionBase _subscription;

        public EventConsumer(string stream, string group)
        {
            _stream = stream;
            _group = group;
            //uncommet to enable verbose logging in client.
            var settings = ConnectionSettings.Create().DisableTls(); //.EnableVerboseLogging().UseConsoleLogger();

            using (_conn = EventStoreConnection.Create(settings, new Uri("tcp://admin:changeit@eventstore_db:1113")))
            //using (_conn = EventStoreConnection.Create(settings, new Uri("tcp://admin:changeit@localhost:1113")))
            {
                _conn.ConnectAsync().Wait();

                CreateSubscription();
                //Call this method for creating a subscription that waits for a new event
                //ConnectToSubscription();

                //Console.WriteLine("waiting for events. press enter to exit");
                //Console.ReadLine();
            }
        }

        public long GetLastEventId()
        {
            try
            {
                var readResult = _conn.ReadEventAsync(_stream, StreamPosition.End, true).Result;
                return readResult.Event.Value.Event.EventNumber;
            }
            catch (Exception)
            {
                return -1;
            }
            
        }

        public void Dispose()
        {
            _conn.Dispose();
        }

        private void ConnectToSubscription()
        {
            var bufferSize = 10;
            var autoAck = true;

            _subscription = _conn.ConnectToPersistentSubscription(_stream, _group, EventAppeared, SubscriptionDropped,
                User, bufferSize, autoAck);
        }

        private void SubscriptionDropped(EventStorePersistentSubscriptionBase eventStorePersistentSubscriptionBase,
            SubscriptionDropReason subscriptionDropReason, Exception ex)
        {
            ConnectToSubscription();
        }

        //Need to wait for this event and somehow notify the main task of its arrival maybe with a flag? Look for better options
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
                _conn.CreatePersistentSubscriptionAsync(_stream, _group, settings, User).Wait();
            }
            catch (AggregateException ex)
            {
                if (ex.InnerException.GetType() != typeof(InvalidOperationException)
                    && ex.InnerException?.Message != $"Subscription group {_group} on stream {_stream} already exists")
                {
                    throw;
                }
            }
        }
    }
}

