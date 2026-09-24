using MediatR;
using SmartClinicQueue.Application.Interfaces.IReposirories;
using SmartClinicQueue.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartClinicQueue.Application.Features.Queue.Commands.CallNextPatient
{
    public class CallNextPatientCommandHandler
        : IRequestHandler<CallNextPatientCommand, int>
    {
        private readonly IQueueTicketRepository _queueTicketRepository;

        public CallNextPatientCommandHandler(
            IQueueTicketRepository queueTicketRepository)
        {
            _queueTicketRepository = queueTicketRepository;
        }

        public async Task<int> Handle(
            CallNextPatientCommand request,
            CancellationToken cancellationToken)
        {
            var currentPatient =
                await _queueTicketRepository
                    .GetCurrentServingAsync(request.DoctorId);

            if (currentPatient is not null)
            {
                throw new InvalidOperationException(
                    "The doctor already has a patient.");
            }

            var queue =
                await _queueTicketRepository
                    .GetActiveQueueByDoctorAsync(request.DoctorId);

            var nextPatient = queue.FirstOrDefault();

            if (nextPatient is null)
            {
                throw new InvalidOperationException(
                    "There are no waiting patients.");
            }

            nextPatient.Status = QueueStatus.Called;
            nextPatient.CalledAt = DateTime.UtcNow;
            nextPatient.UpdatedAt = DateTime.UtcNow;

            _queueTicketRepository.Update(nextPatient);
            await _queueTicketRepository.SaveChangesAsync();

            return nextPatient.Id;
        }
    }
}
