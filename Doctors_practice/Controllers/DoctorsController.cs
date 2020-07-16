using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Doctors_practice.Models.Doctor;

namespace Doctors_practice.Controllers
{
    [ApiController]
    public class DoctorsController : ControllerBase
    {
        private readonly DoctorContext _context;

        public DoctorsController(DoctorContext context)
        {
            _context = context;
        }

        // GET: Doctors
        [HttpGet]
        [Route("Doctors")]
        public async Task<ActionResult<IEnumerable<DoctorDTO>>> GetDoctors()
        {
            return await _context.Doctors.Select(x=>DoctorToDTO(x)).ToListAsync();
        }

        // GET: Doctors/5
        [HttpGet]
        [Route("Doctors/{id}")]
        public async Task<ActionResult<DoctorDTO>> GetDoctor(int id)
        {
            var doctor = await _context.Doctors.FindAsync(id);

            if (doctor == null)
            {
                return NotFound();
            }

            return DoctorToDTO(doctor);
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

            var doctor = await _context.Doctors.FindAsync(id);
            if(doctor == null)
            {
                return NotFound();
            }

            doctor.Name = doctorDTO.Name;
            doctor.Surname = doctorDTO.Surname;
            doctor.Practice_id = doctorDTO.Practice_id;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!DoctorExists(id))
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

        // POST: Doctors
        [HttpPost]
        [Route("Doctors")]
        public async Task<ActionResult<Doctor>> PostDoctor(DoctorDTO doctorDTO)
        {
            var doctor = new Doctor
            {
                Name = doctorDTO.Name,
                Surname = doctorDTO.Surname,
                Practice_id = doctorDTO.Practice_id

            };
            _context.Doctors.Add(doctor);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetDoctor), new { id = doctor.ID }, DoctorToDTO(doctor));
        }

        // DELETE: Doctors/5
        [HttpDelete]
        [Route("Doctors/{id}")]
        public async Task<ActionResult<Doctor>> DeleteDoctor(int id)
        {
            var doctor = await _context.Doctors.FindAsync(id);
            if (doctor == null)
            {
                return NotFound();
            }

            _context.Doctors.Remove(doctor);
            await _context.SaveChangesAsync();

            return doctor;
        }

        private bool DoctorExists(int id)
        {
            return _context.Doctors.Any(e => e.ID == id);
        }

        private static DoctorDTO DoctorToDTO(Doctor doctor) =>
            new DoctorDTO
            {
                ID = doctor.ID,
                Name = doctor.Name,
                Surname = doctor.Surname,
                Practice_id = doctor.Practice_id
            };
    }
}
