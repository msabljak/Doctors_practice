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
using System.Runtime.CompilerServices;

namespace Doctors_practice.Tests
{
    public class ChargePatientHandlerTest
    {
        [Fact]
        public async void Handle_TaskTooLongShouldFail()
        {
            var chargingSystem = new Mock<IDummyChargingSystem>();
            chargingSystem.Setup(cs => cs.SimulateCharge(It.IsAny<double>())).Returns(true);
            var DB = new Mock<IDummyDB>();
            DB.Setup(db => db.PersistTransaction(It.IsAny<double>())).Returns(true);
            var customer = new Mock<ICustomer>();
            customer.SetupGet(x => x.Balance).Returns(500);
           // customer.Setup(x => x.Charge(It.IsAny<double>())).Returns(Task.FromResult(true));            
            ChargePatientHandler chargePatientHandler = new ChargePatientHandler(customer.Object, 5);
            chargePatientHandler.Delay = TimeSpan.FromSeconds(10);
            ChargePatientCommand chargePatientCommand = new ChargePatientCommand(5);
            CancellationToken cancellationToken = new CancellationToken();

            Func<Task> action = () => chargePatientHandler.Handle(chargePatientCommand, cancellationToken);

            await Assert.ThrowsAsync<TimeoutRejectedException>(action);
        }
    }
}
