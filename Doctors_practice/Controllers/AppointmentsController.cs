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
        private readonly AppointmentContext _context;

        public AppointmentsController(AppointmentContext context)
        {
            _context = context;
        }

        // GET: Appointments
        [HttpGet]
        [Route("Appointments")]
        public async Task<ActionResult<IEnumerable<AppointmentDTO>>> GetAppointments()
        {
            return await _context.Appointments.Select(x=>AppointmentToDTO(x)).ToListAsync();
        }

        // GET: Appointments/5
        [HttpGet]
        [Route("Appointments/{id}")]
        public async Task<ActionResult<AppointmentDTO>> GetAppointment(int id)
        {
            var appointment = await _context.Appointments.FindAsync(id);

            if (appointment == null)
            {
                return NotFound();
            }

            return AppointmentToDTO(appointment);
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

            var appointment = await _context.Appointments.FindAsync(id);
            if(appointment == null)
            {
                return NotFound();
            }

            appointment.Doctor_id = appointmentDTO.Doctor_id;
            appointment.Patient_id = appointmentDTO.Patient_id;
            appointment.Reason = appointmentDTO.Reason;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!AppointmentExists(id))
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

        // POST: Appointments
        [HttpPost]
        [Route("Appointments")]
        public async Task<ActionResult<AppointmentDTO>> PostAppointment(AppointmentDTO appointmentDTO)
        {

            var appointment = new Appointment
            {
                Doctor_id=appointmentDTO.Doctor_id,
                Patient_id=appointmentDTO.Patient_id,
                Reason=appointmentDTO.Reason
            };
            _context.Appointments.Add(appointment);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetAppointment), new { id = appointment.ID }, AppointmentToDTO(appointment));
        }

        // DELETE: Appointments/5
        [HttpDelete]
        [Route("Appointments/{id}")]
        public async Task<ActionResult<Appointment>> DeleteAppointment(int id)
        {
            var appointment = await _context.Appointments.FindAsync(id);
            if (appointment == null)
            {
                return NotFound();
            }

            _context.Appointments.Remove(appointment);
            await _context.SaveChangesAsync();

            return appointment;
        }

        private bool AppointmentExists(int id)
        {
            return _context.Appointments.Any(e => e.ID == id);
        }

        private static AppointmentDTO AppointmentToDTO(Appointment appointment) =>
            new AppointmentDTO
            {
                ID = appointment.ID,
                Doctor_id = appointment.Doctor_id,
                Patient_id=appointment.Patient_id,
                Reason=appointment.Reason
            };
    }
}
