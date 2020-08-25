using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Doctors_practice.BusinessLayer
{
    public class DummyDB
    {
        public static bool PersistTransaction(double amount)
        {
            Random random = new Random();
            int randomNumber = random.Next(1, 100);
            if (randomNumber < 50)
            {
                return false;
            }
            return true;
        }
    }
    }
}
