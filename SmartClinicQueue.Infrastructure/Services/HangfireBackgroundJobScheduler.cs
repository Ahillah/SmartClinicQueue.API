using Hangfire;
using SmartClinicQueue.Application.Interfaces.IServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace SmartClinicQueue.Infrastructure.Services
{
    
        public class HangfireBackgroundJobScheduler : IBackgroundJobScheduler
        {
            private readonly IRecurringJobManager _recurringJobManager;

            public HangfireBackgroundJobScheduler(
                IRecurringJobManager recurringJobManager)
            {
                _recurringJobManager = recurringJobManager;
            }
        public void ScheduleDailyQueueCleanup(
    Expression<Func<IQueueCleanupService, Task>> methodCall,
    string cronExpression)
        {
            _recurringJobManager.AddOrUpdate<IQueueCleanupService>(
                "daily-queue-cleanup",
                methodCall,
                cronExpression);
        }
    }
}
