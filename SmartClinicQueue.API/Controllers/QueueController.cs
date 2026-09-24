using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SmartClinicQueue.Application.Common;
using SmartClinicQueue.Application.DTO_s;
using SmartClinicQueue.Application.Features.Queue.Commands.JoinQueue;
using SmartClinicQueue.Application.Features.Queue.Queries.GetMyTicket;
using SmartClinicQueue.Application.Features.Queue.Queries.GetQueue;
using SmartClinicQueue.Domain.Constant;

namespace SmartClinicQueue.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class QueueController : ControllerBase
    {
        private readonly IMediator _mediator;

        public QueueController(IMediator mediator)
        {
            _mediator = mediator;
        }
        [Authorize(Roles = Roles.Receptionist)]
        [HttpPost("join")]
        public async Task<IActionResult> Join([FromBody] JoinQueueCommand command)
        {
            var ticketId = await _mediator.Send(command);
            return Ok(ApiResponse<int>.Success(ticketId, "Ticket created successfully."));
        }
        [Authorize(Roles = Roles.Patient)]
        [HttpGet("my-ticket/{patientId}")]
        public async Task<IActionResult> GetMyTicket(int patientId)
        {
            var result = await _mediator.Send(new GetMyTicketQuery() { PatientId    =patientId});

            if (result is null)
                return NotFound(ApiResponse<string>.Failure(
                    "No active ticket found for this patient."));

            return Ok(ApiResponse<MyTicketDto>.Success(result));
        }
        [Authorize(Roles = Roles.Doctor)]
        [HttpGet("doctor/{doctorId}")]
        public async Task<IActionResult> GetQueue(int doctorId)
        {
            var result = await _mediator.Send(new GetQueueQuery() { DoctorId=doctorId});
            return Ok(ApiResponse<IEnumerable<QueueItemDto>>.Success(result));
        }
    }
}
