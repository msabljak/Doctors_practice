using Apache.NMS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Doctors_practice
{
    public interface IPatientClient
    {
        void SendMessage(string destination, string message);
        Task<ITextMessage> SendMessageAsync(string destination, string message);
        Task<IObjectMessage> SendObjectMessageAsync(string destination, object message);
        Task<MessengerConnectionInfo> SendTransactionalMessageAsync(string destination, string message);
        void CommitTransactionalMessage(IConnection connection, ISession session);
        void RollbackTransactionalMessage(IConnection connection, ISession session);
        string ReadNextMessage(string destination);
    }
}
