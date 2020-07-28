using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace EmailService
{
    public class EmailWorker : BackgroundService
    {
        private readonly ILogger<EmailWorker> _logger;
        public IServiceScopeFactory _serviceScopeFactory;

        public EmailWorker(ILogger<EmailWorker> logger, IServiceScopeFactory serviceScopeFactory)
        {
            _logger = logger;
            _serviceScopeFactory = serviceScopeFactory;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            using (var scope = _serviceScopeFactory.CreateScope())
            {
                while (!stoppingToken.IsCancellationRequested)
                {
                    if (scope.ServiceProvider.GetRequiredService<IClient>().ReadNextMessage("EmailQueue") == "PatientCreated")
                    {
                        _logger.LogInformation("Email confirmation sent!");
                    }
                    await Task.Delay(1000, stoppingToken);
                }
            }            
        }
    }
}
