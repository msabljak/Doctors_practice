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
        public async Task<ActionResult<IEnumerable<Appointment>>> GetAppointments()
        {
            return await _context.Appointments.ToListAsync();
        }

        // GET: Appointments/5
        [HttpGet("{id}")]
        [Route("Appointments/{id}")]
        public async Task<ActionResult<Appointment>> GetAppointment(int id)
        {
            var appointment = await _context.Appointments.FindAsync(id);

            if (appointment == null)
            {
                return NotFound();
            }

            return appointment;
        }

        // PUT: Appointments/5
        [HttpPut("{id}")]
        [Route("Appointments/{id}")]
        public async Task<IActionResult> PutAppointment(int id, Appointment appointment)
        {
            if (id != appointment.ID)
            {
                return BadRequest();
            }

            _context.Entry(appointment).State = EntityState.Modified;

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
        public async Task<ActionResult<Appointment>> PostAppointment(Appointment appointment)
        {
            _context.Appointments.Add(appointment);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetAppointment", new { id = appointment.ID }, appointment);
        }

        // DELETE: Appointments/5
        [HttpDelete("{id}")]
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
    }
}
