using MediatR;
using SmartClinicQueue.Application.DTO_s;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartClinicQueue.Application.Features.Queue.Queries.GetMyTicket
{

    public class GetMyTicketQuery : IRequest<MyTicketDto?>
    {

       public int PatientId { get; set; }   
    }
}


