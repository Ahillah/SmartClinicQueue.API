using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartClinicQueue.Domain.Enums
{
    public enum QueueStatus
    {
        Waiting = 1,
        Called = 2,
        InConsultation = 3,
        Completed = 4,
        Cancelled = 5,
        NoShow = 6
    }
}
