using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartClinicQueue.Application.Features.Queue.Commands.CallNextPatient
{
    public class CallNextPatientCommand: IRequest<int>
    {
        public int DoctorId { get; set; }
    }
}
