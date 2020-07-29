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
        MessengerConnectionInfo SendTransactionalMessage(string destination, string message);
        void CommitTransactionalMessage(IConnection connection, ISession session);
        void RollbackTransactionalMessage(IConnection connection, ISession session);
        string ReadNextMessage(string destination);
    }
}
