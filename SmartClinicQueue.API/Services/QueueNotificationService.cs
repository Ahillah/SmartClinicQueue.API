using Microsoft.AspNetCore.SignalR;
using SmartClinicQueue.API.Hubs;
using SmartClinicQueue.Application.Interfaces.IServices;

namespace SmartClinicQueue.API.Services
{
    public class QueueNotificationService : IQueueNotificationService
    {
        private readonly IHubContext<QueueHub> _hubContext;

        public QueueNotificationService(
            IHubContext<QueueHub> hubContext)
        {
            _hubContext = hubContext;
        }

        public async Task NotifyQueueUpdatedAsync(int doctorId)
        {
            await _hubContext.Clients
                .Group($"doctor-{doctorId}")
                .SendAsync("QueueUpdated", doctorId);
        }
    }
}
