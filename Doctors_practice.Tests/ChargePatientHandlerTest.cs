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
using System.Net.Http;

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
            var customer = new Customer(chargingSystem.Object, DB.Object);
            customer.Balance = 500;
            //customer.SetupGet(x => x.Balance).Returns(500);
            //customer.Setup(x => x.Charge(It.IsAny<double>())).Returns(Task.FromResult(true));            
            ChargePatientHandler chargePatientHandler = new ChargePatientHandler(customer, 1);
            ChargePatientCommand chargePatientCommand = new ChargePatientCommand(5);
            CancellationToken cancellationToken = new CancellationToken();


            Func<Task> action = () => chargePatientHandler.Handle(chargePatientCommand, cancellationToken);
            //Func<Task<bool>> functionWrapper = async () =>
            //{
            //    var res = await chargePatientHandler.Handle(chargePatientCommand, cancellationToken);
            //    return res;
            //};

            await Assert.ThrowsAsync<TimeoutRejectedException>(action);
        }
        [Fact]
        public async void Handle_MockedTaskTooLongShouldFail()
        {
            var customer = new Mock<ICustomer>();
            customer.SetupGet(x => x.Balance).Returns(500);
            customer.Setup(x => x.Charge(It.IsAny<double>())).Returns(async () =>
            {
                await Task.Delay(TimeSpan.FromSeconds(2));
                return true;
            });
            ChargePatientHandler chargePatientHandler = new ChargePatientHandler(customer.Object, 1);
            ChargePatientCommand chargePatientCommand = new ChargePatientCommand(5);
            CancellationToken cancellationToken = new CancellationToken();

            Func<Task> action = () => chargePatientHandler.Handle(chargePatientCommand, cancellationToken);

            await Assert.ThrowsAsync<TimeoutRejectedException>(action);
        }

        [Fact]
        public async void Handle_SingleUncompletedChargeShouldWork()
        {
            var expected = true;
            var customer = new Mock<ICustomer>();
            customer.SetupGet(x => x.Balance).Returns(500);
            customer.SetupSequence(x => x.Charge(It.IsAny<double>())).
                Returns(async () =>
                {
                    await Task.Delay(TimeSpan.FromSeconds(0.5));
                    return false;
                })
                .Returns(async () =>
                {
                    await Task.Delay(TimeSpan.FromSeconds(0.5));
                    return true;
                });
            ChargePatientHandler chargePatientHandler = new ChargePatientHandler(customer.Object, 50, 3);
            ChargePatientCommand chargePatientCommand = new ChargePatientCommand(5);
            CancellationToken cancellationToken = new CancellationToken();

            var actual = await chargePatientHandler.Handle(chargePatientCommand, cancellationToken);

            Assert.Equal(expected, actual);
        }

        [Fact]
        public async void Handle_MultipleUncompletedChargesLessThanMaxRetriesShouldWork()
        {
            var expected = true;
            var customer = new Mock<ICustomer>();
            customer.SetupGet(x => x.Balance).Returns(500);
            customer.SetupSequence(x => x.Charge(It.IsAny<double>()))
                .Returns(async () =>
                {
                    await Task.Delay(TimeSpan.FromSeconds(0.5));
                    return false;
                })
                .Returns(async () =>
                {
                    await Task.Delay(TimeSpan.FromSeconds(0.5));
                    return false;
                })
                .Returns(async () =>
                {
                    await Task.Delay(TimeSpan.FromSeconds(0.5));
                    return true;
                });
            ChargePatientHandler chargePatientHandler = new ChargePatientHandler(customer.Object, 50, 3);
            ChargePatientCommand chargePatientCommand = new ChargePatientCommand(5);
            CancellationToken cancellationToken = new CancellationToken();

            var actual = await chargePatientHandler.Handle(chargePatientCommand, cancellationToken);

            Assert.Equal(expected, actual);
        }

        [Fact]
        public async void Handle_MultipleUncompletedChargersGreaterThanMaxRetriesShouldFail()
        {
            var customer = new Mock<ICustomer>();
            customer.SetupGet(x => x.Balance).Returns(500);
            customer.Setup(x => x.Charge(It.IsAny<double>()))
                .Returns(async () =>
                {
                    await Task.Delay(TimeSpan.FromSeconds(0.5));
                    return false;
                });
            ChargePatientHandler chargePatientHandler = new ChargePatientHandler(customer.Object, 50, 3);
            ChargePatientCommand chargePatientCommand = new ChargePatientCommand(5);
            CancellationToken cancellationToken = new CancellationToken();

            Func<Task> action = () => chargePatientHandler.Handle(chargePatientCommand, cancellationToken);

            await Assert.ThrowsAsync<HttpRequestException>(action);
        }

        [Fact]
        public async void Handle_ValidArgumentsShouldWork()
        {
            var expected = true;
            var customer = new Mock<ICustomer>();
            customer.SetupGet(x => x.Balance).Returns(500);
            customer.SetupSequence(x => x.Charge(It.IsAny<double>()))
                .Returns(async () =>
                {
                    await Task.Delay(TimeSpan.FromSeconds(0.5));
                    return true;
                });
            ChargePatientHandler chargePatientHandler = new ChargePatientHandler(customer.Object, 50, 3);
            ChargePatientCommand chargePatientCommand = new ChargePatientCommand(5);
            CancellationToken cancellationToken = new CancellationToken();

            var actual = await chargePatientHandler.Handle(chargePatientCommand, cancellationToken);

            Assert.Equal(expected, actual);
        }
    }
}
