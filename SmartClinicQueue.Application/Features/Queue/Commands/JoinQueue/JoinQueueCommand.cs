using SmartClinicQueue.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;

namespace SmartClinicQueue.Application.Features.Queue.Commands.JoinQueue
{
    public class JoinQueueCommand : IRequest<int>
    {
        public int PatientId { get; set; }
        public int DoctorId { get; set; }
        public int ClinicId { get; set; }
        public QueuePriority Priority { get; set; }
    }
}
