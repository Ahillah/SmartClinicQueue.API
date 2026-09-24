using MediatR;
using SmartClinicQueue.Application.Interfaces.IReposirories;
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

        public CompleteConsultationCommandHandler(
            IQueueTicketRepository queueTicketRepository)
        {
            _queueTicketRepository = queueTicketRepository;
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

            return true;
        }
    }
}
