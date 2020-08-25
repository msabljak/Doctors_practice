using Doctors_practice.BusinessLayer;
using System;
using Xunit;

namespace Doctors_practice.Tests
{
    public class CustomerTest
    {
        [Fact]
        public void Charge_NegativeAmountShouldFail()
        {            
            //Assert.Throws<ArgumentException>(() => Customer.Charge(-1));
        }
    }
}
