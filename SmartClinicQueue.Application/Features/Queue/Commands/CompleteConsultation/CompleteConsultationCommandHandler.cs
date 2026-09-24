using MediatR;
using SmartClinicQueue.Application.Interfaces.IReposirories;
using SmartClinicQueue.Application.Interfaces.IServices;
using SmartClinicQueue.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartClinicQueue.Application.Features.Queue.Commands.CompleteConsultation
{
    public class CompleteConsultationCommandHandler
       : IRequestHandler<CompleteConsultationCommand, bool>
    {
        private readonly IQueueTicketRepository _queueTicketRepository;
        private readonly IQueueNotificationService _queueNotificationService;
        public CompleteConsultationCommandHandler(
            IQueueTicketRepository queueTicketRepository,
             IQueueNotificationService queueNotificationService
            )
        {
            _queueTicketRepository = queueTicketRepository;
            _queueNotificationService = queueNotificationService;
        }

        public async Task<bool> Handle(
            CompleteConsultationCommand request,
            CancellationToken cancellationToken)
        {
            var ticket = await _queueTicketRepository
              .GetByIdAsync(request.TicketId);

            if (ticket is null)
                throw new KeyNotFoundException("Ticket not found.");

            if (ticket.Status != QueueStatus.InConsultation)
                throw new InvalidOperationException(
                    "Only a patient in consultation can be completed.");

            ticket.Status = QueueStatus.Completed;
            ticket.CompletedAt = DateTime.UtcNow;
            ticket.UpdatedAt = DateTime.UtcNow;

            _queueTicketRepository.Update(ticket);

            await _queueTicketRepository.SaveChangesAsync();
            await _queueNotificationService
    .NotifyQueueUpdatedAsync(ticket.DoctorId);
            return true;
        }
    }
}
