using SmartClinicQueue.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartClinicQueue.Application.DTO_s
{
    public class QueueItemDto
    {
        public int TicketId { get; set; }
        public int TicketNumber { get; set; }
        public int PatientId { get; set; }
        public QueuePriority Priority { get; set; }
        public DateTime CreatedAt { get; set; }
        public int Position { get; set; }
    }
}
