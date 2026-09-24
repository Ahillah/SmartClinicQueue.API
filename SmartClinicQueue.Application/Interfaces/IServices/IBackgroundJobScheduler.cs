using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace SmartClinicQueue.Application.Interfaces.IServices
{
    public interface IBackgroundJobScheduler
    {
        void ScheduleDailyQueueCleanup(
               Expression<Func<IQueueCleanupService, Task>> methodCall,
               string cronExpression);
    }
}
