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
        private IPracticeRepository _practiceRepository;

        public PracticesController(IPracticeRepository practiceRepository)
        {
            _practiceRepository = practiceRepository;
        }

        // GET: Practices
        [HttpGet]
        [Route("Practices")]
        public IEnumerable<PracticeDTO> GetPractices()
        {
            return _practiceRepository.GetAllPractices();
        }

        // GET: Practices/5
        [HttpGet]
        [Route("Practices/{id}")]
        public PracticeDTO GetPractice(int id)
        {
            return _practiceRepository.GetPractices(id);
        }

        // PUT: Practices/5
        [HttpPut]
        [Route("Practices/{id}")]
        public async Task<IActionResult> PutPractice(int id, PracticeDTO practiceDTO)
        {
            if (id != practiceDTO.ID)
            {
                return BadRequest();
            }

            if (_practiceRepository.Update(practiceDTO,id)==0)
            {
                return NotFound();
            }

            return NoContent();
        }

        // POST: Practices
        [HttpPost]
        [Route("Practices")]
        public async Task<ActionResult<PracticeDTO>> PostPractice(PracticeDTO practiceDTO)
        {
            var practice = _practiceRepository.Add(practiceDTO);

            return CreatedAtAction(nameof(GetPractice), new { id = practice.ID }, practice);
        }

        // DELETE: Practices/5
        [HttpDelete]
        [Route("Practices/{id}")]
        public async Task<ActionResult<Practices>> DeletePractice(int id)
        {
            if (_practiceRepository.Delete(id) == 0)
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
