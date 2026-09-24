using MediatR;

using SmartClinicQueue.Application.Interfaces.IReposirories;
using SmartClinicQueue.Application.Interfaces.IServices;
using SmartClinicQueue.Domain.Entities;
using SmartClinicQueue.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace SmartClinicQueue.Application.Features.Queue.Commands.JoinQueue
{
   
        public class JoinQueueCommandHandler
            : IRequestHandler<JoinQueueCommand, int>
        {
            private readonly IQueueTicketRepository _queueTicketRepository;
        private readonly IQueueNotificationService _queueNotificationService;

        public JoinQueueCommandHandler(IQueueTicketRepository queueTicketRepository,
            IQueueNotificationService queueNotificationService)
            {
                _queueTicketRepository = queueTicketRepository;
                _queueNotificationService = queueNotificationService;   
            }

            public async Task<int> Handle(
                JoinQueueCommand request,
                CancellationToken cancellationToken)
            {
                var activeTicket = await _queueTicketRepository
                    .GetActiveTicketByPatientAsync(request.PatientId);

                if (activeTicket is not null)
                    throw new InvalidOperationException(
                        "You already have an active ticket.");

            var lastTicketNumber = await _queueTicketRepository
  .GetLastTicketNumberAsync(request.DoctorId);

            var ticketNumber = lastTicketNumber + 1;


            var ticket = new QueueTicket
                {
                    TicketNumber = ticketNumber,
                    PatientId = request.PatientId,
                    DoctorId = request.DoctorId,
                    ClinicId = request.ClinicId,
                    Priority = request.Priority,
                    Status = QueueStatus.Waiting,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                await _queueTicketRepository.AddAsync(ticket);
                await _queueTicketRepository.SaveChangesAsync();
            await _queueNotificationService
    .NotifyQueueUpdatedAsync(request.DoctorId);

            return ticket.Id;
            }
        }
    }
