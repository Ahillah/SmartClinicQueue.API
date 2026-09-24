using MediatR;
using SmartClinicQueue.Application.Interfaces.IReposirories;
using SmartClinicQueue.Application.Interfaces.IServices;
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
        private readonly IQueueNotificationService _queueNotificationService;

        public StartConsultationCommandHandler(
            IQueueTicketRepository queueTicketRepository,
             IQueueNotificationService queueNotificationService)
        {
            _queueTicketRepository = queueTicketRepository;
            _queueNotificationService = queueNotificationService;
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
            await _queueNotificationService
.NotifyQueueUpdatedAsync(ticket.DoctorId);
            return true;
        }
    }
}
