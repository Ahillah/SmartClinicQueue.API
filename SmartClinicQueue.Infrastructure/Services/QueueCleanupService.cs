using SmartClinicQueue.Application.Interfaces.IReposirories;
using SmartClinicQueue.Application.Interfaces.IServices;
using SmartClinicQueue.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartClinicQueue.Infrastructure.Services
{
    public class QueueCleanupService : IQueueCleanupService
    {
        private readonly IQueueTicketRepository _queueTicketRepository;

        public QueueCleanupService(
            IQueueTicketRepository queueTicketRepository)
        {
            _queueTicketRepository = queueTicketRepository;
        }

        public async Task CleanupExpiredQueueAsync()
        {
            var waitingTickets =
                await _queueTicketRepository.GetWaitingTicketsFromPreviousDaysAsync();

            foreach (var ticket in waitingTickets)
            {
                ticket.Status = QueueStatus.NoShow;
                ticket.UpdatedAt = DateTime.UtcNow;

                _queueTicketRepository.Update(ticket);
            }
            await _queueTicketRepository.SaveChangesAsync();
        }
        }
}
