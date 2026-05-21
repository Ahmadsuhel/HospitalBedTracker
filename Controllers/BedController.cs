using HospitalBedTracker.Application.Commands.UpdateBedCount;
using HospitalBedTracker.Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace HospitalBedTracker.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BedController : ControllerBase
    {
        private readonly IMediator _mediator;

        public BedController(IMediator mediator)
        {
            _mediator = mediator;
        }

        // Hospital staff bed count update kare
        [HttpPut("update")]
        public async Task<IActionResult> UpdateBedCount(
            [FromBody] UpdateBedRequest request)
        {
            var command = new UpdateBedCountCommand(
                request.HospitalId,
                request.BedType,
                request.TotalBeds,
                request.AvailableBeds,
                null
            );

            var result = await _mediator.Send(command);

            if (!result.Success)
                return BadRequest(new { result.Message });

            return Ok(new { result.Message, result.UpdatedAt });
        }

        // Multiple bed types ek saath update
        [HttpPost("bulk-update")]
        public async Task<IActionResult> BulkUpdate(
            [FromBody] BulkUpdateRequest request)
        {
            var results = new List<object>();

            foreach (var bed in request.Beds)
            {
                var result = await _mediator.Send(new UpdateBedCountCommand(
                    request.HospitalId,
                    bed.BedType,
                    bed.TotalBeds,
                    bed.AvailableBeds,
                    null));

                results.Add(new
                {
                    bed.BedType,
                    result.Success,
                    result.Message
                });
            }

            return Ok(results);
        }
    }

    // Request models
    public record UpdateBedRequest(
        Guid HospitalId,
        BedType BedType,
        int TotalBeds,
        int AvailableBeds
    );

    public record BulkUpdateRequest(
        Guid HospitalId,
        List<BedUpdateItem> Beds
    );

    public record BedUpdateItem(
        BedType BedType,
        int TotalBeds,
        int AvailableBeds
    );
}