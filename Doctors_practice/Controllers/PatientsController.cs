using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Doctors_practice.Models;

namespace Doctors_practice.Controllers
{
    
    [ApiController]
    public class PatientsController : ControllerBase
    {
        private readonly PatientContext _context;

        public PatientsController(PatientContext context)
        {
            _context = context;
        }

        // GET: Patients
        [HttpGet]
        [Route("Patients")]
        public async Task<ActionResult<IEnumerable<PatientDTO>>> GetPatients()
        {
            return await _context.Patients.Select(x=>PatientToDTO(x)).ToListAsync();
        }

        // GET: Patients/5
        [HttpGet("{id}")]
        [Route("Patients/{id}")]
        public async Task<ActionResult<PatientDTO>> GetPatient(int id)
        {
            var patient = await _context.Patients.FindAsync(id);

            if (patient == null)
            {
                return NotFound();
            }

            return PatientToDTO(patient);
        }

        // PUT: Patients/5
        [HttpPut("{id}")]
        [Route("Patients/{id}")]
        public async Task<IActionResult> PutPatient(int id, PatientDTO patientDTO)
        {
            if (id != patientDTO.ID)
            {
                return BadRequest();
            }

            var patient = await _context.Patients.FindAsync(id);
            if(patient == null)
            {
                return NotFound();
            }

            patient.Name = patientDTO.Name;
            patient.Surname = patientDTO.Surname;
            patient.Telephone = patientDTO.Telephone;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PatientExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: Patients
        [HttpPost]
        [Route("Patients")]
        public async Task<ActionResult<PatientDTO>> PostPatient(PatientDTO patientDTO)
        {
            var patient = new Patient
            {
                Name = patientDTO.Name,
                Surname = patientDTO.Surname,
                Telephone = patientDTO.Telephone
            };

            _context.Patients.Add(patient);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetPatient), new { id = patient.ID }, PatientToDTO(patient));
        }

        // DELETE: Patients/5
        [HttpDelete("{id}")]
        [Route("Patients/{id}")]
        public async Task<ActionResult<Patient>> DeletePatient(int id)
        {
            var patient = await _context.Patients.FindAsync(id);
            if (patient == null)
            {
                return NotFound();
            }

            _context.Patients.Remove(patient);
            await _context.SaveChangesAsync();

            return patient;
        }

        private bool PatientExists(int id)
        {
            return _context.Patients.Any(e => e.ID == id);
        }

        private static PatientDTO PatientToDTO(Patient patient) =>
            new PatientDTO
            {
                ID = patient.ID,
                Name = patient.Name,
                Surname = patient.Surname,
                Telephone = patient.Telephone
            };
    }
}
