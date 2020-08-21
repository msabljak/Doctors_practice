using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;

namespace Doctors_practice.Models.Patient
{
    [Serializable]
    public class PatientMessage : PatientDTO
    {        
        public string EventType { get; set; }
        public PatientMessage()
        {

        }
        public PatientMessage(string eventType, PatientDTO patientDTO)
        {
            ID = patientDTO.ID;
            Name = patientDTO.Name;
            Surname = patientDTO.Surname;
            Telephone = patientDTO.Telephone;
            EventType = eventType;
        }
    }
}
