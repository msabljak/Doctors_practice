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
        public async Task<ActionResult<IEnumerable<PracticeDTO>>> GetPractices()
        {
            return await _context.Practices.Select(x=>PracticeToDTO(x)).ToListAsync();
        }

        // GET: Practices/5
        [HttpGet]
        [Route("Practices{id}")]
        public async Task<ActionResult<PracticeDTO>> GetPractice(int id)
        {
            var practice = await _context.Practices.FindAsync(id);

            if (practice == null)
            {
                return NotFound();
            }

            return PracticeToDTO(practice);
        }

        // PUT: Practices/5
        [HttpPut]
        [Route("Practices{id}")]
        public async Task<IActionResult> PutPractice(int id, PracticeDTO practiceDTO)
        {
            if (id != practiceDTO.ID)
            {
                return BadRequest();
            }

            var practice = await _context.Practices.FindAsync(id);
            if(practice == null)
            {
                return NotFound();
            }

            practice.Name = practiceDTO.Name;
            practice.Address = practiceDTO.Address;

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

        // POST: Practices
        [HttpPost]
        [Route("Practices")]
        public async Task<ActionResult<PracticeDTO>> PostPractice(PracticeDTO practiceDTO)
        {
            var practice = new Practice
            {
                Name = practiceDTO.Name,
                Address = practiceDTO.Address
            };

            _context.Practices.Add(practice);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetPractice), new { id = practice.ID }, PracticeToDTO(practice));
        }

        // DELETE: Practices/5
        [HttpDelete]
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

        private static PracticeDTO PracticeToDTO(Practice practice) =>
            new PracticeDTO
            {
                ID = practice.ID,
                Name = practice.Name,
                Address = practice.Address
            };

    }
}
