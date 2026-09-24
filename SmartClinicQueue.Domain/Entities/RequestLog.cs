using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartClinicQueue.Domain.Entities
{
   
        public class RequestLog : BaseEntity<int>
        {
            public string HttpMethod { get; set; } = null!;

            public string Url { get; set; } = null!;

            public string? Headers { get; set; }

            public string? IpAddress { get; set; }

            public int StatusCode { get; set; }

            public long ResponseTimeMs { get; set; }

            public int? UserId { get; set; }

            public ApplicationUser? User { get; set; }
        }
    }
