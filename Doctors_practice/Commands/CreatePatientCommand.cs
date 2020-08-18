using Doctors_practice.Models;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Doctors_practice.Commands
{
    public class CreatePatientCommand : IRequest<PatientDTO>
    {
        public PatientDTO PatientDTO { get; }
        public CreatePatientCommand(PatientDTO patientDTO)
        {
            PatientDTO = patientDTO;
        }
    }
}
