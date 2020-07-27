using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Doctors_practice
{
    public interface IPatientClient
    {
        void SendMessage(string message);
        string ReadNextMessage();
    }
}
