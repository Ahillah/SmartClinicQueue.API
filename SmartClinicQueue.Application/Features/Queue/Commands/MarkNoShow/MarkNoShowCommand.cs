using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartClinicQueue.Application.Features.Queue.Commands.MarkNoShow
{
    public class MarkNoShowCommand : IRequest<bool>
    {
        public int TicketId { get; set; }
    }
}
