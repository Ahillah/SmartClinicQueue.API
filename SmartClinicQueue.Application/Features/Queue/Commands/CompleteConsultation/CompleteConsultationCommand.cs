using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartClinicQueue.Application.Features.Queue.Commands.CompleteConsultation
{
    public class CompleteConsultationCommand : IRequest<bool>
    {
        public int TicketId { get; set; }
    }
}
