using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartClinicQueue.Application.Features.Queue.Commands.StartConsultation
{
    public class StartConsultationCommand : IRequest<bool>
    {
        public int TicketId { get; set; }
    }
}
