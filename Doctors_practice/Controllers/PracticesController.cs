using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Doctors_practice.Models.Practice;

namespace Doctors_practice.Controllers
{

    [ApiController]
    public class PracticesController : ControllerBase
    {
        private readonly PracticeContext _context;

        public PracticesController(PracticeContext context)
        {
            _context = context;
        }

        // GET: Practices
        [HttpGet]
        [Route("Practices")]
        public async Task<ActionResult<IEnumerable<Practice>>> GetPractices()
        {
            return await _context.Practices.ToListAsync();
        }

        // GET: Practices/5
        [HttpGet("{id}")]
        [Route("Practices{id}")]
        public async Task<ActionResult<Practice>> GetPractice(int id)
        {
            var practice = await _context.Practices.FindAsync(id);

            if (practice == null)
            {
                return NotFound();
            }

            return practice;
        }

        // PUT: Practices/5
        [HttpPut("{id}")]
        [Route("Practices{id}")]
        public async Task<IActionResult> PutPractice(int id, Practice practice)
        {
            if (id != practice.ID)
            {
                return BadRequest();
            }

            _context.Entry(practice).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PracticeExists(id))
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

        // POST: api/Practices
        [HttpPost]
        [Route("Practices")]
        public async Task<ActionResult<Practice>> PostPractice(Practice practice)
        {
            _context.Practices.Add(practice);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetPractice", new { id = practice.ID }, practice);
        }

        // DELETE: Practices/5
        [HttpDelete("{id}")]
        [Route("Practices{id}")]
        public async Task<ActionResult<Practice>> DeletePractice(int id)
        {
            var practice = await _context.Practices.FindAsync(id);
            if (practice == null)
            {
                return NotFound();
            }

            _context.Practices.Remove(practice);
            await _context.SaveChangesAsync();

            return practice;
        }

        private bool PracticeExists(int id)
        {
            return _context.Practices.Any(e => e.ID == id);
        }
    }
}
