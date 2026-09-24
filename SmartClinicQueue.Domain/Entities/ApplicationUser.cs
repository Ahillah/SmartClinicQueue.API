using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace SmartClinicQueue.Domain.Entities
{
      public class ApplicationUser : IdentityUser<int>
        {
            public string FirstName { get; set; } = null!;
            public string LastName { get; set; } = null!;

            
            public Doctor? Doctor { get; set; }

            public ICollection<QueueTicket> QueueTickets { get; set; }
                = new List<QueueTicket>();

    }
    }

