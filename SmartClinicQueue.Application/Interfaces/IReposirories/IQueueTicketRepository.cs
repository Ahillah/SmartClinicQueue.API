using SmartClinicQueue.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartClinicQueue.Application.Interfaces.IReposirories
{
    public interface IQueueTicketRepository : IGenericRepository<QueueTicket, int>
    {
        Task<QueueTicket?> GetActiveTicketByPatientAsync(int patientId);

        Task<int> GetLastTicketNumberAsync(int doctorId);

        Task<IEnumerable<QueueTicket>> GetActiveQueueByDoctorAsync(int doctorId);

        Task<QueueTicket?> GetCurrentServingAsync(int doctorId);

        Task<IEnumerable<QueueTicket>>
    GetWaitingTicketsFromPreviousDaysAsync();
    }
}
