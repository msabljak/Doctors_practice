using System;

namespace Doctors_practice.BusinessLayer
{
    public class DummyChargingSystem
    {
        public DummyChargingSystem()
        {

        }
        public bool SimulateCharge(double amount)
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