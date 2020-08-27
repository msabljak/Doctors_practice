using System;
using System.Collections.Generic;
using System.Text;
using Xunit;
using Moq;
using Doctors_practice.BusinessLayer;
using Doctors_practice.CommandHandlers;
using Doctors_practice.Commands;
using System.Threading;
using Polly.Timeout;
using System.Threading.Tasks;

namespace Doctors_practice.Tests
{
    public class ChargePatientHandlerTest
    {
        [Fact]
        public void Handle_TaskTooLongShouldFail()
        {
            double charge = 300;
            var chargingSystem = new Mock<IDummyChargingSystem>();
            chargingSystem.Setup(cs => cs.SimulateCharge(It.IsAny<double>())).Returns(true);
            var DB = new Mock<IDummyDB>();
            DB.Setup(db => db.PersistTransaction(It.IsAny<double>())).Returns(false);
            var customer = new Mock<ICustomer>();
            customer.SetupGet(x => x.Balance).Returns(400);
            customer.Setup(x => x.Charge(charge, It.IsAny<TimeSpan>())).Returns(Task.FromResult(true));
            ChargePatientHandler chargePatientHandler = new ChargePatientHandler(customer.Object, 3);
            ChargePatientCommand chargePatientCommand = new ChargePatientCommand(5);
            CancellationToken cancellationToken = new CancellationToken();

            Action action = async () => await chargePatientHandler.Handle(chargePatientCommand, cancellationToken);

            Assert.Throws<TimeoutRejectedException>(action);
        }
    }
}
