using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SmartClinicQueue.Application.Common;
using SmartClinicQueue.Application.DTO_s;
using SmartClinicQueue.Application.Features.Queue.Commands.CallNextPatient;
using SmartClinicQueue.Application.Features.Queue.Commands.CancelTicket;
using SmartClinicQueue.Application.Features.Queue.Commands.CompleteConsultation;
using SmartClinicQueue.Application.Features.Queue.Commands.JoinQueue;
using SmartClinicQueue.Application.Features.Queue.Commands.MarkNoShow;
using SmartClinicQueue.Application.Features.Queue.Commands.StartConsultation;
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

        [Authorize(Roles = Roles.Doctor)]
        [HttpPost("doctor/{doctorId}/call-next")]
        public async Task<IActionResult> CallNext(int doctorId)
        {
            var ticketId = await _mediator.Send(
                new CallNextPatientCommand() { DoctorId=doctorId});

            return Ok(
                ApiResponse<int>.Success(
                    ticketId,
                    "Next patient called successfully."));
        }
        [Authorize(Roles = Roles.Doctor)]
        [HttpPost("{ticketId}/start-consultation")]
        public async Task<IActionResult> StartConsultation(int ticketId)
        {
            await _mediator.Send(
                new StartConsultationCommand() { TicketId=ticketId});

            return Ok(
                ApiResponse<bool>.Success(
                    true,
                    "Consultation started successfully."));
        }
        [Authorize(Roles = Roles.Doctor)]
        [HttpPost("{ticketId}/complete-consultation")]
        public async Task<IActionResult> CompleteConsultation(int ticketId)
        {
            await _mediator.Send(
                new CompleteConsultationCommand() { TicketId= ticketId});

            return Ok(
                ApiResponse<bool>.Success(
                    true,
                    "Consultation completed successfully."));
        }
        [Authorize(Roles = Roles.Receptionist)]
        [HttpPost("{ticketId}/cancel")]
        public async Task<IActionResult> CancelTicket(int ticketId)
        {
            await _mediator.Send(
                new CancelTicketCommand(){ TicketId = ticketId });

            return Ok(
                ApiResponse<bool>.Success(
                    true,
                    "Ticket cancelled successfully."));
        }
        [Authorize(Roles = Roles.Doctor)]
        [HttpPost("{ticketId}/no-show")]
        public async Task<IActionResult> MarkNoShow(int ticketId)
        {
            await _mediator.Send(
                new MarkNoShowCommand() { TicketId = ticketId });

            return Ok(
                ApiResponse<bool>.Success(
                    true,
                    "Patient marked as no-show successfully."));
        }
    }
}
