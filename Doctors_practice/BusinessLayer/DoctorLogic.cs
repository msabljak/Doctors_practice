using Doctors_practice.Models;
using Doctors_practice.Models.Appointment;
using Doctors_practice.Models.Doctor;
using Doctors_practice.Models.Patient;
using Doctors_practice.Models.Practice;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Doctors_practice.BusinessLayer
{
    public class DoctorLogic
    {
        private DoctorDTO doctor;
        private IEnumerable<PatientDTO> patients;
        private IEnumerable<AppointmentDTO> appointments;

        public IEnumerable<AppointmentDTO> Appointments
        {
            get { return appointments; }
            set { appointments = value; }
        }


        public IEnumerable<PatientDTO> Patients
        {
            get { return patients; }
            set { patients = value; }
        }

        public DoctorDTO Doctor
        {
            get { return doctor; }
            set { doctor = value; }
        }

        public DoctorLogic()
        {

        }
        public DoctorLogic(DoctorDTO doctor, IEnumerable<PatientDTO> patients, IEnumerable<AppointmentDTO> appointments)
        {
            Doctor = doctor;
            Patients = patients;
            Appointments = appointments;
        }

        //Maybe change Patients property to list of doctors Patients and send list of general patients in method parametar instead
        public IEnumerable<PatientDTO> GetDoctorPatients()
        {
            //List<int> patientID = new List<int>();
            List<PatientDTO> doctorPatients = new List<PatientDTO>();
            //bool exists = false;
            //foreach (AppointmentDTO appointment in Appointments)
            //{
            //    if (appointment.Doctor_id == Doctor.ID)
            //    {
            //        patientID.Add(appointment.Patient_id);
            //    }
            //}
            //foreach (PatientDTO patient in Patients)
            //{
            //    if (patientID.Contains(patient.ID))
            //    {
            //        exists = false;
            //        foreach (PatientDTO doctorPatient in doctorPatients)
            //        {
            //            if (doctorPatient.ID == patient.ID)
            //            {
            //                exists = true;
            //            }
            //        }
            //        if (exists == false)
            //        {
            //            doctorPatients.Add(patient);
            //        }
            //    }
            //}
            var query = from patient in Patients
                        join appointment in Appointments
                        on patient.ID equals appointment.Patient_id
                        where appointment.Doctor_id == doctor.ID
                        select patient;
            foreach (var patient in query)
            {
                doctorPatients.Add(patient);
            }
            return doctorPatients;
        }
    }
}
