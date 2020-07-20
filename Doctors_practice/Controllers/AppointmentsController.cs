using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Doctors_practice.Models.Appointment;

namespace Doctors_practice.Controllers
{
    [ApiController]
    public class AppointmentsController : ControllerBase
    {
        private IAppointmentRepository _appointmentRepository;

        public AppointmentsController(IAppointmentRepository appointmentRepository)
        {
            _appointmentRepository = appointmentRepository;
        }

        // GET: Appointments
        [HttpGet]
        [Route("Appointments")]
        public IEnumerable<AppointmentDTO> GetAppointments()
        {
            return  _appointmentRepository.GetAllAppointments();
        }

        // GET: Appointments/5
        [HttpGet]
        [Route("Appointments/{id}")]
        public AppointmentDTO GetAppointment(int id)
        {
            return _appointmentRepository.GetAppointment(id);
        }

        // PUT: Appointments/5
        [HttpPut]
        [Route("Appointments/{id}")]
        public async Task<IActionResult> PutAppointment(int id, AppointmentDTO appointmentDTO)
        {
            if (id != appointmentDTO.ID)
            {
                return BadRequest();
            }
            int updateResult = _appointmentRepository.Update(appointmentDTO, id);
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

        // POST: Appointments
        [HttpPost]
        [Route("Appointments")]
        public async Task<ActionResult<AppointmentDTO>> PostAppointment(AppointmentDTO appointmentDTO)
        {
            var doctor = _appointmentRepository.Add(appointmentDTO);
            if (doctor == null)
            {
                return BadRequest();
            }
            return CreatedAtAction(nameof(GetAppointment), new { id = doctor.ID }, doctor);
        }

        // DELETE: Appointments/5
        [HttpDelete]
        [Route("Appointments/{id}")]
        public async Task<ActionResult<Appointments>> DeleteAppointment(int id)
        {
            if (_appointmentRepository.Delete(id) == 0)
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
