using MediatR;
using SmartClinicQueue.Application.DTO_s;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartClinicQueue.Application.Features.Queue.Queries.GetQueue
{
  
    public class GetQueueQuery : IRequest<IEnumerable<QueueItemDto>>
    {
        public int DoctorId { get; set; }
    }

}
