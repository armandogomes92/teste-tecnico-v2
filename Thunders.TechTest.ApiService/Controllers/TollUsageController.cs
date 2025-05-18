using Microsoft.AspNetCore.Mvc;
using Thunders.TechTest.ApiService.DTOs.Requests;
using Thunders.TechTest.ApiService.Messages; // Added this line
using Thunders.TechTest.OutOfBox.Queues;
using System.Threading.Tasks;

namespace Thunders.TechTest.ApiService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TollUsageController : ControllerBase
    {
        private readonly IMessageSender _messageSender;

        public TollUsageController(IMessageSender messageSender)
        {
            _messageSender = messageSender;
        }

        [HttpPost("ingest")]
        public async Task<IActionResult> IngestData([FromBody] TollUsageDataRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var message = new TollUsageDataMessage
            {
                Timestamp = request.Timestamp,
                TollPlaza = request.TollPlaza,
                City = request.City,
                State = request.State,
                AmountPaid = request.AmountPaid,
                VehicleType = (VehicleTypeMessage)request.VehicleType // Enum cast
            };

            await _messageSender.SendLocal(message);

            return Accepted();
        }
    }
}