using SmartClinicQueue.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartClinicQueue.Domain.Entities
{
   
        public class QueueTicket :BaseEntity<int>   
    {
         

            public int TicketNumber { get; set; }

            public int PatientId { get; set; }

            public int DoctorId { get; set; }

            public int ClinicId { get; set; }

            public QueuePriority Priority { get; set; }

            public QueueStatus Status { get; set; }

          

            public DateTime? CalledAt { get; set; }

            public DateTime? StartedAt { get; set; }

            public DateTime? CompletedAt { get; set; }

            // Navigation
            public ApplicationUser Patient { get; set; } = null!;

            public Doctor Doctor { get; set; } = null!;

            public Clinic Clinic { get; set; } = null!;
        }
    }
