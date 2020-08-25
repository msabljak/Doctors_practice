using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Doctors_practice.BusinessLayer
{
    public interface IDummyDB
    {
        bool PersistTransaction(double amount);
    }
}
