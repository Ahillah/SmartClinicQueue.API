using MediatR;
using SmartClinicQueue.Application.DTO_s;
using SmartClinicQueue.Application.Interfaces.IReposirories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartClinicQueue.Application.Features.Queue.Queries.GetQueue
{
  
        public class GetQueueQueryHandler
            : IRequestHandler<GetQueueQuery, IEnumerable<QueueItemDto>>
        {
            private readonly IQueueTicketRepository _queueTicketRepository;

            public GetQueueQueryHandler(IQueueTicketRepository queueTicketRepository)
            {
                _queueTicketRepository = queueTicketRepository;
            }

            public async Task<IEnumerable<QueueItemDto>> Handle(
                GetQueueQuery request,
                CancellationToken cancellationToken)
            {
                var queue = await _queueTicketRepository
                    .GetActiveQueueByDoctorAsync(request.DoctorId);

                var result = queue
                    .Select((t, index) => new QueueItemDto
                    {
                        TicketId = t.Id,
                        TicketNumber = t.TicketNumber,
                        PatientId = t.PatientId,
                        Priority = t.Priority,
                        CreatedAt = t.CreatedAt,
                        Position = index + 1
                    })
                    .ToList();

                return result;
            }
        }
    }
