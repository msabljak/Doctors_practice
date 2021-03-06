﻿using EventStore.ClientAPI;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventStoreImplementation
{
    public class EventProducer : IDisposable
    {
        private IEventStoreConnection _connection;
        private IConfiguration _configuration;

        public EventProducer(IConfiguration configuration)
        {
            _configuration = configuration;
            //uncomment to enable verbose logging in client.
            var settings = ConnectionSettings.Create().DisableTls();//.EnableVerboseLogging().UseConsoleLogger();
            //_connection = EventStoreConnection.Create(settings, new Uri("tcp://admin:changeit@localhost:1113"));
            _connection = EventStoreConnection.Create(settings, new Uri(_configuration.GetConnectionString("eventstore")));
            _connection.ConnectAsync()/*.Wait()*/;
        }

        public void SendEvent(string stream, EventData eventData)
        {
            _connection.AppendToStreamAsync(stream, ExpectedVersion.Any, eventData).Wait();
        }

        public void Dispose()
        {
            _connection.Dispose();
        }

        public EventData CreateEventData(object sampleObject, string eventType, byte[] metadata = null)
        {
            var data = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(sampleObject));
            if (metadata == null)
            {
                metadata = Encoding.UTF8.GetBytes("{}");
            }
            var eventPayload = new EventData(Guid.NewGuid(), eventType, true, data, metadata);
            return eventPayload;
        }
        //private static EventData GetEventDataFor(int i)
        //{
        //    return new EventData(
        //        Guid.NewGuid(),
        //        "eventType",
        //        true,
        //        Encoding.ASCII.GetBytes("{'somedata' : " + i + "}"),
        //        Encoding.ASCII.GetBytes("{'metadata' : " + i + "}")
        //        );
        //}
    }
}
