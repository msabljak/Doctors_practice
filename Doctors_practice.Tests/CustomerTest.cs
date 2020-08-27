using Doctors_practice.BusinessLayer;
using System;
using Xunit;
using Moq;

namespace Doctors_practice.Tests
{
    public class CustomerTest
    {
        [Fact]
        public void Charge_NegativeAmountShouldFail()
        {
            var chargingSystem = new Mock<IDummyChargingSystem>();
            var DB = new Mock<IDummyDB>();
            var customer = new Customer(chargingSystem.Object, DB.Object);
            string expected = "Amount to be charged can not be a negative value!";

            Action action = () => customer.Charge(-1, new TimeSpan(hours: 0, minutes: 0, seconds: 0));

            var actual = Assert.Throws<ArgumentException>(action);
            Assert.Equal(expected,actual.Message);
        }

        [Fact]
        public void Charge_AmountEqualToZeroShouldFail()
        {
            var chargingSystem = new Mock<IDummyChargingSystem>();
            var DB = new Mock<IDummyDB>();
            var customer = new Customer(chargingSystem.Object, DB.Object);
            string expected = "Have to charge customer something!";

            Action action = () => customer.Charge(0, new TimeSpan(hours: 0, minutes: 0, seconds: 0));

            var actual = Assert.Throws<ArgumentException>(action);
            Assert.Equal(expected, actual.Message);
        }

        [Fact]
        public void Charge_AmountLargerThanBalanceShouldFail()
        {
            var chargingSystem = new Mock<IDummyChargingSystem>();
            var DB = new Mock<IDummyDB>();
            var customer = new Customer(chargingSystem.Object, DB.Object);
            customer.Balance = 400;
            string expected = "There is not sufficient funds on the balance to support this charge!";

            Action action = () => customer.Charge(500, new TimeSpan(hours: 0, minutes: 0, seconds: 0));

            var actual = Assert.Throws<ArgumentException>(action);
            Assert.Equal(expected, actual.Message);
        }

        [Fact]
        public void Charge_UnsuccesfulChargeSystemCallShouldFail()
        {
            double charge = 300;
            var chargingSystem = new Mock<IDummyChargingSystem>();
            chargingSystem.Setup(cs => cs.SimulateCharge(It.IsAny<double>())).Returns(false);
            var DB = new Mock<IDummyDB>();
            var customer = new Customer(chargingSystem.Object, DB.Object);
            customer.Balance = 400;
            string expected = "3rd party Charging System failed to charge!";

            Action action = () => customer.Charge(charge, new TimeSpan(hours: 0, minutes: 0, seconds: 0));

            var actual = Assert.Throws<ArgumentException>(action);
            Assert.Equal(expected, actual.Message);
        }

        [Fact]
        public void Charge_UnsuccesfulPersistTransactionCallShouldFail()
        {
            double charge = 300;
            var chargingSystem = new Mock<IDummyChargingSystem>();
            chargingSystem.Setup(cs => cs.SimulateCharge(It.IsAny<double>())).Returns(true);
            var DB = new Mock<IDummyDB>();
            DB.Setup(db => db.PersistTransaction(It.IsAny<double>())).Returns(false);
            var customer = new Customer(chargingSystem.Object, DB.Object);
            customer.Balance = 400;
            string expected = "Transaction failed!";

            Action action = () => customer.Charge(charge, new TimeSpan(hours: 0, minutes: 0, seconds: 0));

            var actual = Assert.Throws<ArgumentException>(action);
            Assert.Equal(expected, actual.Message);
        }

        [Fact]
        public void Charge_ValidArgumentsShouldWork()
        {
            double charge = 300;
            var chargingSystem = new Mock<IDummyChargingSystem>();
            chargingSystem.Setup(cs => cs.SimulateCharge(It.IsAny<double>())).Returns(true);
            var DB = new Mock<IDummyDB>();
            DB.Setup(db => db.PersistTransaction(It.IsAny<double>())).Returns(true);
            var customer = new Customer(chargingSystem.Object,DB.Object);
            customer.Balance = 400;

            var actual = customer.Charge(charge, new TimeSpan(hours: 0, minutes: 0, seconds: 0)).Result;

            Assert.True(actual != false);
        }
    }
}
