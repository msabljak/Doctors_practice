using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Doctors_practice.Models.Doctor;
using Doctors_practice.Models;
using Doctors_practice.Models.Appointment;
using Doctors_practice.Models.Patient;
using Doctors_practice.BusinessLayer;
using Doctors_practice.Services;
using Newtonsoft.Json;
using Microsoft.Extensions.Configuration;

namespace Doctors_practice.Controllers
{
    [ApiController]
    public class DoctorsController : ControllerBase
    {
        private IDoctorRepository _doctorRepository;
        private IAppointmentRepository _appointmentRepository;
        private IPatientRepository _patientRepository;
        private ICacheService _cacheService;
        private IConfiguration _configuration;

        public DoctorsController(IDoctorRepository doctorRepository, IAppointmentRepository appointmentRepository, IPatientRepository patientRepository, ICacheService cacheService, IConfiguration configuration)
        {
            _doctorRepository = doctorRepository;
            _appointmentRepository = appointmentRepository;
            _patientRepository = patientRepository;
            _cacheService = cacheService;
            _configuration = configuration;
        }

        // GET: Doctors
        [HttpGet]
        [Route("Doctors")]
        public async Task<IEnumerable<DoctorDTO>> GetDoctors()
        {
            if (_configuration.GetValue<string>("Properties:cacheEnabled")=="true")
            {
                if (await _cacheService.GetCacheValueAsync(Request.Path) == null)
                {
                    IEnumerable<DoctorDTO> doctorDTOs = _doctorRepository.GetAllDoctors();
                    string doctorDTOsAsJSON = JsonConvert.SerializeObject(doctorDTOs);
                    await _cacheService.SetCacheValueAsync(Request.Path, doctorDTOsAsJSON);
                    return doctorDTOs;
                }
                else
                {
                    return JsonConvert.DeserializeObject<IEnumerable<DoctorDTO>>(await _cacheService.GetCacheValueAsync(Request.Path));
                }
            }
            else 
            {
                return _doctorRepository.GetAllDoctors();
            }
        }

        // GET: Doctors/5
        [HttpGet]
        [Route("Doctors/{id}")]
        public async Task<DoctorDTO> GetDoctor(int id)
        {
            if (_configuration.GetValue<string>("Properties:cacheEnabled") == "true")
            {
                var cacheValue = await _cacheService.GetCacheValueAsync(Request.Path);
                if (cacheValue == null)
                {
                    DoctorDTO doctorDTO = _doctorRepository.GetDoctor(id);
                    string doctorDTOAsJSON = JsonConvert.SerializeObject(doctorDTO);
                    await _cacheService.SetCacheValueAsync(Request.Path, doctorDTOAsJSON);
                    return doctorDTO;
                }
                else
                {
                    return JsonConvert.DeserializeObject<DoctorDTO>(cacheValue);
                }
            }
            return _doctorRepository.GetDoctor(id);
        }

        // GET: Doctors/5/Patients
        [HttpGet]
        [Route("Doctors/{id}/Patients")]
        public IEnumerable<PatientDTO> GetDoctorPatients(int id)
        {
            DoctorDTO doctor = _doctorRepository.GetDoctor(id);
            IEnumerable<PatientDTO> patients = _patientRepository.GetAllPatients();
            IEnumerable<AppointmentDTO> appointments = _appointmentRepository.GetAllAppointments();
            DoctorLogic doctorLogic = new DoctorLogic(doctor, patients, appointments);
            IEnumerable<PatientDTO> doctorsPatients = doctorLogic.GetDoctorPatients();
            return doctorsPatients;
        }

        // PUT: Doctors/5
        [HttpPut]
        [Route("Doctors/{id}")]
        public async Task<IActionResult> PutDoctor(int id, DoctorDTO doctorDTO)
        {
            if (id != doctorDTO.ID)
            {
                return BadRequest();
            }
            int updateResult = _doctorRepository.Update(doctorDTO, id);
            if ( updateResult == 0)
            {
                return NotFound();
            }
            else if (updateResult == -1)
            {
                return BadRequest();
            }

            return NoContent();
        }

        // POST: Doctors
        [HttpPost]
        [Route("Doctors")]
        public async Task<ActionResult<Doctors>> PostDoctor(DoctorDTO doctorDTO)
        {
            var doctor = _doctorRepository.Add(doctorDTO);

            return CreatedAtAction(nameof(GetDoctor), new { id = doctor.ID }, doctor);
        }

        // DELETE: Doctors/5
        [HttpDelete]
        [Route("Doctors/{id}")]
        public async Task<ActionResult<Doctors>> DeleteDoctor(int id)
        {
            if (_doctorRepository.Delete(id) == 0)
            {
                return NotFound();
            }
            else
            {
                return Ok();
            }
        }  
    }
}
