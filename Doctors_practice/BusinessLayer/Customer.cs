using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Doctors_practice.BusinessLayer
{
    public class Customer : ICustomer
    {
        private IDummyChargingSystem _dummyChargingSystem;
        private IDummyDB _dummyDB;

        public string Name { get; set; }
        public double Balance { get; set; }
        public Customer(IDummyChargingSystem dummyChargingSystem, IDummyDB dummyDB)
        {
            _dummyChargingSystem = dummyChargingSystem;
            _dummyDB = dummyDB;
        }

        public async Task<bool> Charge(double amount)
        {
            await Task.Delay(TimeSpan.FromSeconds(10));
            if (double.IsNegative(amount))
            {
                throw new ArgumentException("Amount to be charged can not be a negative value!");
            }
            else if (amount == 0)
            {
                throw new ArgumentException("Have to charge customer something!");
            }
            else if (Balance <= amount)
            {
                throw new ArgumentException("There is not sufficient funds on the balance to support this charge!");
            }
            bool chargingResult = _dummyChargingSystem.SimulateCharge(amount);
            if (chargingResult == false)
            {
                throw new ArgumentException("3rd party Charging System failed to charge!");
            }
            bool transactionResult = _dummyDB.PersistTransaction(amount);
            if (transactionResult == false)
            {
                throw new ArgumentException("Transaction failed!");
            }
            return true;
        }
    }
}
