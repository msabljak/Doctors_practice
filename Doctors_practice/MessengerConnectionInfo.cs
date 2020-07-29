using Apache.NMS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Doctors_practice
{
    public class MessengerConnectionInfo
    {
        public IConnection Connection { get; set; }
        public ISession Session { get; set; }

        public MessengerConnectionInfo(IConnection connection, ISession session)
        {
            Connection = connection;
            Session = session;
        }
    }
}
