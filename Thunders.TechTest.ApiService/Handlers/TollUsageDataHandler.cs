using Rebus.Handlers;
using System.Threading.Tasks;
using Thunders.TechTest.ApiService.Data;
using Thunders.TechTest.ApiService.Messages;
using Thunders.TechTest.ApiService.Models;
using Microsoft.Extensions.Logging;

namespace Thunders.TechTest.ApiService.Handlers
{
    public class TollUsageDataHandler : IHandleMessages<TollUsageDataMessage>
    {
        private readonly AppDbContext _dbContext;
        private readonly ILogger<TollUsageDataHandler> _logger;

        public TollUsageDataHandler(AppDbContext dbContext, ILogger<TollUsageDataHandler> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }

        public async Task Handle(TollUsageDataMessage message)
        {
            _logger.LogInformation("Processing TollUsageDataMessage for TollPlaza: {TollPlaza}, City: {City}", message.TollPlaza, message.City);

            var tollUsageData = new TollUsageData
            {
                Timestamp = message.Timestamp,
                TollPlaza = message.TollPlaza,
                City = message.City,
                State = message.State,
                AmountPaid = message.AmountPaid,
                VehicleType = (VehicleTypeData)message.VehicleType // Enum cast
            };

            _dbContext.TollUsages.Add(tollUsageData);
            await _dbContext.SaveChangesAsync();

            _logger.LogInformation("Successfully processed and saved TollUsageDataMessage with Id: {Id}", tollUsageData.Id);
        }
    }
}