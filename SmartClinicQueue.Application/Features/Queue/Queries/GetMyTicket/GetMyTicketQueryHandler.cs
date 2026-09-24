using MediatR;
using SmartClinicQueue.Application.DTO_s;
using SmartClinicQueue.Application.Interfaces.IReposirories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartClinicQueue.Application.Features.Queue.Queries.GetMyTicket
{
    public class GetMyTicketQueryHandler
            : IRequestHandler<GetMyTicketQuery, MyTicketDto?>
        {
            private readonly IQueueTicketRepository _queueTicketRepository;

            public GetMyTicketQueryHandler(IQueueTicketRepository queueTicketRepository)
            {
                _queueTicketRepository = queueTicketRepository;
            }

            public async Task<MyTicketDto?> Handle(
                GetMyTicketQuery request,
                CancellationToken cancellationToken)
            {
                
                var ticket = await _queueTicketRepository
                    .GetActiveTicketByPatientAsync(request.PatientId);

                if (ticket is null)
                    return null;

               
                var queue = (await _queueTicketRepository
                    .GetActiveQueueByDoctorAsync(ticket.DoctorId))
                    .ToList();

                var currentServing = await _queueTicketRepository
                    .GetCurrentServingAsync(ticket.DoctorId);

         
                var position = queue.FindIndex(t => t.Id == ticket.Id) + 1;

                return new MyTicketDto
                {
                    TicketId = ticket.Id,
                    TicketNumber = ticket.TicketNumber,
                    Priority = ticket.Priority,
                    Status = ticket.Status,
                    Position = position,
                    PatientsBeforeYou = position - 1,
                    NowServingTicketNumber = currentServing?.TicketNumber
                };
            }
        }
    }

