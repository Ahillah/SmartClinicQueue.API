using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartClinicQueue.Application.Interfaces.IServices
{
    public interface IQueueCleanupService
    {
        Task CleanupExpiredQueueAsync();
    }
}
