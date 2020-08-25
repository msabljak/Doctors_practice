using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Doctors_practice.BusinessLayer
{
    public class Customer
    {
        public string Name { get; set; }
        public double Balance { get; set; }

        public void Charge(double amount)
        {
            if (double.IsNegative(amount))
            {
                throw new ArgumentException("Amount to be charged can not be a negative value!");
            }
            else if (amount != 0)
            {
                throw new ArgumentException("Have to charge customer something!");
            }
            else if (Balance <= amount)
            {
                throw new ArgumentException("There is not sufficient funds on the balance to support this charge!");
            }
            DummyChargingSystem dummyChargingSystem = new DummyChargingSystem();
            bool chargingResult = dummyChargingSystem.SimulateCharge(amount);
            if (chargingResult == false)
            {
                throw new ArgumentException("3rd party Charging System failed to charge!");
            }
            DummyDB dummyDB = new DummyDB();
            bool transactionResult = dummyDB.PersistTransaction(amount);
            if (transactionResult == false)
            {
                throw new ArgumentException("Transaction failed!");
            }
        }
    }
}
