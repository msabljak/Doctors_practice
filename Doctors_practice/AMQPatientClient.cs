using Apache.NMS;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Doctors_practice
{
    public class AMQPatientClient : IPatientClient
    {
        private static Uri _brokerUri;
        private IConfiguration _configuration;
        public AMQPatientClient(IConfiguration configuration)
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

        public Task<ITextMessage> SendMessageAsync(string destination, string message)
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
                    var textMessage = session.CreateTextMessage(message);
                    producer.Send(textMessage);
                    return Task.FromResult(textMessage);
                    //Console.WriteLine($"Sent {message} messages");
                }
            }
        }
        public Task<MessengerConnectionInfo> SendTransactionalMessageAsync(string destination, string message)
        {
            NMSConnectionFactory factory = new NMSConnectionFactory(_brokerUri);
            IConnection connection = factory.CreateConnection();
            connection.Start();

            ISession session = connection.CreateSession(AcknowledgementMode.Transactional);
            MessengerConnectionInfo messengerConnectionInfo = new MessengerConnectionInfo(connection, session);
            using (IDestination dest = session.GetQueue(destination))
            using (IMessageProducer producer = session.CreateProducer(dest))
            {
                producer.DeliveryMode = MsgDeliveryMode.NonPersistent;

                producer.Send(session.CreateTextMessage(message));
            }
            connection.Stop();
            return Task.FromResult(messengerConnectionInfo);
        }

        public void CommitTransactionalMessage(IConnection connection, ISession session)
        {
            connection.Start();
            session.Commit();
            session.Dispose();
            connection.Close();
            connection.Dispose();
        }
            
        public void RollbackTransactionalMessage(IConnection connection, ISession session)
        {
            connection.Start();
            session.Rollback();
            session.Dispose();
            connection.Close();
            connection.Dispose();
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
