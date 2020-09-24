using Doctors_practice.Models;
using Doctors_practice.Models.Appointment;
using Doctors_practice.Models.Doctor;
using Doctors_practice.Models.Patient;
using Doctors_practice.Models.Practice;
using Doctors_practice.Querys;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Nest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Doctors_practice.QueryHandlers
{
    public class UpdateElasticsearchIndexHandler : IRequestHandler<UpdateElasticsearchIndexQuery, IActionResult>
    {
        private IAppointmentRepository _appointmentRepository;
        private IDoctorRepository _doctorRepository;
        private IPatientRepository _patientRepository;
        private IElasticClient _elasticClient;

        public UpdateElasticsearchIndexHandler(IAppointmentRepository appointmentRepository, IDoctorRepository doctorRepository,IPatientRepository patientRepository, IElasticClient elasticClient)
        {
            _appointmentRepository = appointmentRepository;
            _doctorRepository = doctorRepository;
            _patientRepository = patientRepository;
            _elasticClient = elasticClient;
        }
        public async Task<IActionResult> Handle(UpdateElasticsearchIndexQuery request, CancellationToken cancellationToken)
        {
            List<AppointmentDTO> appointmentDTOs = _appointmentRepository.GetAllAppointments().ToList();
            List<DoctorDTO> doctorDTOs = _doctorRepository.GetAllDoctors().ToList();
            List<PatientDTO> patientDTOs = _patientRepository.GetAllPatients().ToList();

            await _elasticClient.IndexManyAsync(appointmentDTOs, "appointments");
            await _elasticClient.IndexManyAsync(doctorDTOs, "doctors");
            await _elasticClient.IndexManyAsync(patientDTOs, "patients");

            return new StatusCodeResult(200);
        }
    }
}
