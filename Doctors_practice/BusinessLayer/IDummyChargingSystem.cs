using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Doctors_practice.BusinessLayer
{
    public interface IDummyChargingSystem
    {
        bool SimulateCharge(double amount);
    }
}
