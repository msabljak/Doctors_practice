using Doctors_practice.BusinessLayer;
using Doctors_practice.Commands;
using MediatR;
using Microsoft.AspNetCore.Http;
using Polly;
using Polly.Retry;
using Polly.Timeout;
using Polly.Wrap;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Doctors_practice.CommandHandlers
{
    public class ChargePatientHandler : IRequestHandler<ChargePatientCommand, bool>
    {
        private readonly AsyncTimeoutPolicy _timeoutPolicy;
        private readonly AsyncPolicyWrap _policyWrap;
        private ICustomer _customer;
        public int MaxRetries { get; set; }
        public int TimeoutInSeconds { get; set; }
        public TimeSpan Delay { get; set; }
        public ChargePatientHandler(ICustomer customer, int timeoutInSeconds)
        {
            TimeoutInSeconds = timeoutInSeconds;
            _timeoutPolicy = Policy.TimeoutAsync(TimeoutInSeconds, TimeoutStrategy.Pessimistic);
            var _retryPolicy = Policy.Handle<HttpRequestException>().RetryAsync(MaxRetries);
            //_policyWrap = Policy.WrapAsync(_timeoutPolicy, _retryPolicy);
            _customer = customer;
        }
        public async Task<bool> Handle(ChargePatientCommand request, CancellationToken cancellationToken)
        {
            return await _timeoutPolicy.ExecuteAsync(async () => 
            {
                ////Does not work, function is never actually ever called so the delay within does not activate
                //await _customer.Charge(300);
                //Does work and will call timeout exception if configured in a manner it should
                await Task.Delay(Delay);
                //if (result == false)
                //{
                //    throw new HttpRequestException("This is a fake request Exception");
                //}
                return true;
            });
        }
    }
}
