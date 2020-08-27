using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Doctors_practice.BusinessLayer
{
    public interface ICustomer
    {
        string Name { get; set; }
        double Balance { get; set; }
        Task<bool> Charge(double amount, TimeSpan sleepTime);
    }
}
