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
        private IDoctorRepository _doctorRepository;

        public DoctorsController(IDoctorRepository doctorRepository)
        {
            _doctorRepository = doctorRepository;
        }

        // GET: Doctors
        [HttpGet]
        [Route("Doctors")]
        public IEnumerable<DoctorDTO> GetDoctors()
        {
            return _doctorRepository.GetAllDoctors();
        }

        // GET: Doctors/5
        [HttpGet]
        [Route("Doctors/{id}")]
        public DoctorDTO GetDoctor(int id)
        {
            return _doctorRepository.GetDoctor(id);
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

            if (_doctorRepository.Update(doctorDTO, id) == 0)
            {
                return NotFound();
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
