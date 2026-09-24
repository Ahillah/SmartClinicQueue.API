using MediatR;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartClinicQueue.Application.Features.Queue.Commands.CancelTicket
{
    public class CancelTicketCommand :IRequest<bool>
    {
       public int TicketId { get; set; }

    }
}
