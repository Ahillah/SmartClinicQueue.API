using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace SmartClinicQueue.Domain.Entities
{
      public class Clinic: BaseEntity<int>
        {
           

            public string Name { get; set; } = null!;

            public string? Description { get; set; }

            public bool IsActive { get; set; } = true;

          
            public ICollection<Doctor> Doctors { get; set; }
                = new List<Doctor>();

            public ICollection<QueueTicket> QueueTickets { get; set; }
                = new List<QueueTicket>();
        }
    }

