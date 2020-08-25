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
            var chargingSystem = new Mock<DummyChargingSystem>();
            var DB = new Mock<DummyDB>();
            var customer = new Customer(chargingSystem.Object, DB.Object);
            string expected = "Amount to be charged can not be a negative value!";

            Action action = () => customer.Charge(-1);

            var actual = Assert.Throws<ArgumentException>(action);
            Assert.Equal(expected,actual.Message);
        }

        [Fact]
        public void Charge_AmountEqualToZeroShouldFail()
        {
            var chargingSystem = new Mock<DummyChargingSystem>();
            var DB = new Mock<DummyDB>();
            var customer = new Customer(chargingSystem.Object, DB.Object);
            string expected = "Have to charge customer something!";

            Action action = () => customer.Charge(0);

            var actual = Assert.Throws<ArgumentException>(action);
            Assert.Equal(expected, actual.Message);
        }

        [Fact]
        public void Charge_AmountLargerThanBalanceShouldFail()
        {
            var chargingSystem = new Mock<DummyChargingSystem>();
            var DB = new Mock<DummyDB>();
            var customer = new Customer(chargingSystem.Object, DB.Object);
            customer.Balance = 400;
            string expected = "There is not sufficient funds on the balance to support this charge!";

            Action action = () => customer.Charge(500);

            var actual = Assert.Throws<ArgumentException>(action);
            Assert.Equal(expected, actual.Message);
        }

        [Fact]
        public void Charge_UnsuccesfulChargeSystemCallShouldFail()
        {
            double charge = 300;
            var chargingSystem = new Mock<DummyChargingSystem>();
            chargingSystem.Setup(cs => cs.SimulateCharge(It.IsAny<double>())).Returns(false);

            var DB = new Mock<DummyDB>();
            var customer = new Customer(chargingSystem.Object, DB.Object);
            customer.Balance = 400;
            string expected = "3rd party Charging System failed to charge!";

            Action action = () => customer.Charge(charge);

            var actual = Assert.Throws<ArgumentException>(action);
            Assert.Equal(expected, actual.Message);
        }

        [Fact]
        public void Charge_UnsuccesfulPersistTransactionCallShouldFail()
        {
            var DB = new Mock<DummyDB>();
            var customer = new Mock<Customer>();
            string expected = "Transaction failed!";

            Action action = () => customer.Object.Charge(300);

            var actual = Assert.Throws<ArgumentException>(action);
            Assert.Equal(expected, actual.Message);
        }

        [Fact]
        public void Charge_ValidArgumentsShouldWork()
        {
            var chargingSystem = new Mock<DummyChargingSystem>();
            var DB = new Mock<DummyDB>();
            var customer = new Mock<Customer>();

            var actual = customer.Object.Charge(300);

            Assert.True(actual != false);
        }
    }
}
