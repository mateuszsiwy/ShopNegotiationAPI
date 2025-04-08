using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using ShopNegotiationAPI.Application.Interfaces.Services;
using ShopNegotiationAPI.Domain.Models;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace ShopNegotiationAPI.Infrastructure.BackgroundServices;

public class ExpiredNegotiationsService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<ExpiredNegotiationsService> _logger;

    public ExpiredNegotiationsService(IServiceProvider serviceProvider, ILogger<ExpiredNegotiationsService> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var negotiationService = scope.ServiceProvider.GetRequiredService<INegotiationService>();

                try
                {
                    var pendingNegotiations = await negotiationService.GetPendingNegotiationsAsync();
                    foreach (var negotiation in pendingNegotiations)
                    {
                        if (negotiation.ExpirationDate < DateTime.UtcNow)
                        {
                            negotiation.Status = NegotiationStatus.Expired;
                            await negotiationService.UpdateNegotiationAsync(negotiation);
                            _logger.LogInformation("Negotiation {Id} marked as expired", negotiation.Id);
                        }
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error processing expired negotiations");
                }
            }

            await Task.Delay(TimeSpan.FromHours(1), stoppingToken);
        }
    }
}
