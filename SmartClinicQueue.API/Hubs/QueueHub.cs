using Microsoft.AspNetCore.SignalR;

namespace SmartClinicQueue.API.Hubs
{
    public class QueueHub : Hub
    {
     
        public async Task JoinDoctorQueue(int doctorId)
        {
            await Groups.AddToGroupAsync(
                Context.ConnectionId,
                $"doctor-{doctorId}");
        }

        public async Task LeaveDoctorQueue(int doctorId)
        {
            await Groups.RemoveFromGroupAsync(
                Context.ConnectionId,
                $"doctor-{doctorId}");
        }

    }
}
