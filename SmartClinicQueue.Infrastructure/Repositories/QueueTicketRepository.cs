using Microsoft.EntityFrameworkCore;
using SmartClinicQueue.Application.Interfaces.IReposirories;
using SmartClinicQueue.Domain.Entities;
using SmartClinicQueue.Domain.Enums;
using SmartClinicQueue.Infrastructure.Persistance;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartClinicQueue.Infrastructure.Repositories
{
    public class QueueTicketRepository
        : GenericRepository<QueueTicket, int>, IQueueTicketRepository
    {
        public QueueTicketRepository(ApplicationDbContext context)
            : base(context)
        {
        }
        public async Task<QueueTicket?> GetActiveTicketByPatientAsync(int patientId)
        {
            return await _dbSet
              
                .FirstOrDefaultAsync(t =>
                    t.PatientId == patientId &&
                    (t.Status == QueueStatus.Waiting ||
                     t.Status == QueueStatus.Called ||
                     t.Status == QueueStatus.InConsultation));
        }

        public async Task<int> GetLastTicketNumberAsync(int doctorId)
        {
            var today = DateTime.UtcNow.Date;
            var tomorrow = today.AddDays(1);

            return await _dbSet
                .AsNoTracking()
                .Where(t =>
                    t.DoctorId == doctorId &&
                    t.CreatedAt >= today &&
                    t.CreatedAt < tomorrow)
                .Select(t => (int?)t.TicketNumber)
                .MaxAsync() ?? 0;
        }
        public async Task<IEnumerable<QueueTicket>> GetActiveQueueByDoctorAsync(int doctorId)
        {
            return await _dbSet
                .AsNoTracking()
                .Where(t =>
                    t.DoctorId == doctorId &&
                    t.Status == QueueStatus.Waiting)
                .OrderByDescending(t => t.Priority)  
                .ThenBy(t => t.CreatedAt)           
                .ToListAsync();
        }

        public async Task<QueueTicket?> GetCurrentServingAsync(int doctorId)
        {
            return await _dbSet
                .AsNoTracking()
                .FirstOrDefaultAsync(t =>
                    t.DoctorId == doctorId &&
                    (t.Status == QueueStatus.Called ||
                     t.Status == QueueStatus.InConsultation));
        }
    }
}
