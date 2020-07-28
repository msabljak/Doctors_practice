using System;
using System.Collections.Generic;
using System.Text;

namespace EmailService
{
    public interface IClient
    {
        void SendMessage(string destination, string message);
        string ReadNextMessage(string destination);
    }
}
