using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartClinicQueue.Domain.Entities
{
     public class Doctor :BaseEntity<int>
        {
          

            public int UserId { get; set; }

            public int ClinicId { get; set; }

            public string Specialization { get; set; } = null!;

            public bool IsActive { get; set; } = true;

          
            public ApplicationUser User { get; set; } = null!;

            public Clinic Clinic { get; set; } = null!;

            public ICollection<QueueTicket> QueueTickets { get; set; }
                = new List<QueueTicket>();
        }
    }

