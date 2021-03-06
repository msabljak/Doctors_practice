﻿using Apache.NMS;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace EmailService
{
    class AMQClient : IClient
    {
        private IConfiguration _configuration;
        private static Uri _brokerUri;

        public AMQClient(IConfiguration configuration)
        {
            _configuration = configuration;
            _brokerUri = new Uri(_configuration.GetConnectionString("amq"));
            //_brokerUri = new Uri("activemq:tcp://localhost:61616");
        }

        public void SendMessage(string destination, string message)
        {
            NMSConnectionFactory factory = new NMSConnectionFactory(_brokerUri);
            using (IConnection connection = factory.CreateConnection())
            {
                connection.Start();

                using (ISession session = connection.CreateSession(AcknowledgementMode.AutoAcknowledge))
                using (IDestination dest = session.GetQueue(destination))
                using (IMessageProducer producer = session.CreateProducer(dest))
                {
                    producer.DeliveryMode = MsgDeliveryMode.NonPersistent;

                    producer.Send(session.CreateTextMessage(message));
                    //Console.WriteLine($"Sent {message} messages");
                }
            }
        }
        public string ReadNextMessage(string destination)
        {
            NMSConnectionFactory factory = new NMSConnectionFactory(_brokerUri);
            using (IConnection connection = factory.CreateConnection())
            {
                connection.Start();
                using (ISession session = connection.CreateSession(AcknowledgementMode.AutoAcknowledge))
                using (IDestination dest = session.GetQueue(destination))
                using (IMessageConsumer consumer = session.CreateConsumer(dest))
                {
                    IMessage msg = consumer.Receive();
                    if (msg is ITextMessage)
                    {
                        ITextMessage txtMsg = msg as ITextMessage;
                        string body = txtMsg.Text;
                        //if (body == "PatientCreated")
                        //{
                            
                        //}
                        //Console.WriteLine($"Received message: {txtMsg.Text}");

                        return body;
                    }
                    else
                    {
                        //Console.WriteLine("Unexpected message type: " + msg.GetType().Name);
                        return "Unexpected message type: " + msg.GetType().Name;
                    }
                }
            }
        }

        public object ReadNextObjectMessage(string destination)
        {
            NMSConnectionFactory factory = new NMSConnectionFactory(_brokerUri);
            using (IConnection connection = factory.CreateConnection())
            {
                connection.Start();
                using (ISession session = connection.CreateSession(AcknowledgementMode.AutoAcknowledge))
                using (IDestination dest = session.GetQueue(destination))
                using (IMessageConsumer consumer = session.CreateConsumer(dest))
                {
                    IMessage msg = consumer.Receive();
                    if (msg is IObjectMessage)
                    {
                        IObjectMessage objMsg = msg as IObjectMessage;
                        var body = objMsg.Body;
                        //if (body == "PatientCreated")
                        //{

                        //}
                        //Console.WriteLine($"Received message: {txtMsg.Text}");

                        return body;
                    }
                    else
                    {
                        //Console.WriteLine("Unexpected message type: " + msg.GetType().Name);
                        return "Unexpected message type: " + msg.GetType().Name;
                    }
                }
            }
        }
    }
}
