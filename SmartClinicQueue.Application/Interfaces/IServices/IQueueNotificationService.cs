using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartClinicQueue.Application.Interfaces.IServices
{
    public interface IQueueNotificationService
    {
        Task NotifyQueueUpdatedAsync(int doctorId);
    }
}
