using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Doctors_practice.BusinessLayer
{
    public class Customer
    {
        public static string Name { get; set; }
        public static double Balance { get; set; }

        public static void Charge(double amount)
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
            bool chargingResult = DummyChargingSystem.SimulateCharge(amount);
            if (chargingResult == false)
            {
                throw new ArgumentException("3rd party Charging System failed to charge!");
            }
            bool transactionResult = DummyDB.PersistTransaction(amount);
            if (transactionResult == false)
            {
                throw new ArgumentException("Transaction failed!");
            }
        }
    }
}
