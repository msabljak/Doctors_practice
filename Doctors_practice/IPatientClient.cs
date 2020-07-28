﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Doctors_practice
{
    public interface IPatientClient
    {
        void SendMessage(string destination, string message);
        string ReadNextMessage(string destination);
    }
}
