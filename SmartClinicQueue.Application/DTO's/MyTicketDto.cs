using SmartClinicQueue.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartClinicQueue.Application.DTO_s
{
    public class MyTicketDto
    {
        public int TicketId { get; set; }
        public int TicketNumber { get; set; }
        public QueuePriority Priority { get; set; }
        public QueueStatus Status { get; set; }

        public int Position { get; set; }
        public int PatientsBeforeYou { get; set; }
        public int? NowServingTicketNumber { get; set; }
    }
}
