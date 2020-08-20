using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace EmailService
{
    public class EmailWorker : BackgroundService
    {
        private readonly ILogger<EmailWorker> _logger;
        public IClient _client;

        public EmailWorker(ILogger<EmailWorker> logger, IClient client, IConfiguration configuration)
        {
            _logger = logger;
            _client = client;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                if (_client.ReadNextMessage("EmailQueue") == "PatientCreated")
                {
                    _logger.LogInformation("Email confirmation sent!");
                }
                await Task.Delay(1000, stoppingToken);
            }
        }
    }
}
