using MediatR;
using SmartClinicQueue.Application.Interfaces.IReposirories;
using SmartClinicQueue.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartClinicQueue.Application.Features.Queue.Commands.StartConsultation
{
    public class StartConsultationCommandHandler
         : IRequestHandler<StartConsultationCommand, bool>
    {
        private readonly IQueueTicketRepository _queueTicketRepository;

        public StartConsultationCommandHandler(
            IQueueTicketRepository queueTicketRepository)
        {
            _queueTicketRepository = queueTicketRepository;
        }

        public async Task<bool> Handle(
            StartConsultationCommand request,
            CancellationToken cancellationToken)
        {
            var ticket = await _queueTicketRepository
               .GetByIdAsync(request.TicketId);

            if (ticket is null)
                throw new KeyNotFoundException("Ticket not found.");

            if (ticket.Status != QueueStatus.Called)
                throw new InvalidOperationException(
                    "Only a called patient can start consultation.");

            ticket.Status = QueueStatus.InConsultation;
            ticket.StartedAt = DateTime.UtcNow;
            ticket.UpdatedAt = DateTime.UtcNow;

            _queueTicketRepository.Update(ticket);

            await _queueTicketRepository.SaveChangesAsync();
            return true;
        }
    }
}
