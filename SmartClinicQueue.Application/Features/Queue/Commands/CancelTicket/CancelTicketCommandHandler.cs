using MediatR;
using SmartClinicQueue.Application.Interfaces.IReposirories;
using SmartClinicQueue.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartClinicQueue.Application.Features.Queue.Commands.CancelTicket
{
    public class CancelTicketCommandHandler
      : IRequestHandler<CancelTicketCommand, bool>
    {
        private readonly IQueueTicketRepository _queueTicketRepository;

        public CancelTicketCommandHandler(
            IQueueTicketRepository queueTicketRepository)
        {
            _queueTicketRepository = queueTicketRepository;
        }
        public async Task<bool> Handle(
         CancelTicketCommand request,
         CancellationToken cancellationToken)
        {
            var ticket = await _queueTicketRepository
                .GetByIdAsync(request.TicketId);

            if (ticket is null)
                throw new KeyNotFoundException("Ticket not found.");

            if (ticket.Status != QueueStatus.Waiting)
                throw new InvalidOperationException(
                    "Only a waiting ticket can be cancelled.");

            ticket.Status = QueueStatus.Cancelled;
            ticket.UpdatedAt = DateTime.UtcNow;

            _queueTicketRepository.Update(ticket);

            await _queueTicketRepository.SaveChangesAsync();

            return true;
        }
    }
}
