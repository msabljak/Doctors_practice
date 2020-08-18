using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Doctors_practice.Models;
using Microsoft.Data.SqlClient;
using System.Data;
using Doctors_practice.Models.Patient;
using MediatR;
using Doctors_practice.Commands;

namespace Doctors_practice.Controllers
{
    
    [ApiController]
    public class PatientsController : ControllerBase
    {
        private IPatientRepository _patientRepository;
        private IPatientClient _client;
        private IMediator _mediator;

        public PatientsController(IPatientRepository patientRepository, IPatientClient client, IMediator mediator)
        {
            _patientRepository = patientRepository;
            _client = client;
            _mediator = mediator;
        }
        // GET: Patients
        [HttpGet]
        [Route("Patients")]
        
        public IEnumerable<PatientDTO> GetPatients()
        {
            return _patientRepository.GetAllPatients();    
        }

        // GET: Patients/5
        [HttpGet]
        [Route("Patients/{id}")]
        public PatientDTO GetPatient(int id)
        {
            return _patientRepository.GetPatients(id);
        }

        // GET: Patients/server
        [HttpGet]
        [Route("Patients/server")]
        public string GetServerID()
        {
            HttpRequest httpRequest = HttpContext.Request;
            return $"Http Response Information:{Environment.NewLine}" +
                                   $"Schema:{httpRequest.Scheme} \n" +
                                   $"Host: {httpRequest.Host} \n" +
                                   $"Path: {httpRequest.Path} \n" +
                                   $"TraceID: {HttpContext.TraceIdentifier} \n" +
                                   $"QueryString: {httpRequest.QueryString} ";
        }

        // PUT: Patients/5
        [HttpPut]
        [Route("Patients/{id}")]
        public async Task<IActionResult> PutPatient(int id, PatientDTO patientDTO)
        {
            if (id != patientDTO.ID)
            {
                return BadRequest();
            }
            if(_patientRepository.Update(patientDTO, id)==0)
            {
                return NotFound();
            }

            return NoContent();
        }

        //POST: Patients
       [HttpPost]
       [Route("Patients")]
        public async Task<ActionResult<PatientDTO>> PostPatient(PatientDTO patientDTO)
        {
            var result = await _mediator.Send(new CreatePatientCommand(patientDTO));
            if (result == null)
            {
                return StatusCode(500);
            }
            return CreatedAtAction(nameof(GetPatient), new { id = result.ID }, result);
        }

        //[HttpPost]
        //[Route("Patients")]
        //public async Task<ActionResult> PostPatient(PatientDTO patientDTO)
        //{
        //    Transaction transaction = new Transaction(_client, _patientRepository);
        //    if (transaction.ExecuteTransaction(patientDTO, "EmailQueue", "PatientCreated")==false){
        //        return BadRequest();
        //    }
        //    return Ok();
        //}

        // DELETE: Patients/5
        [HttpDelete("{id}")]
        [Route("Patients/{id}")]
        public async Task<ActionResult<Patients>> DeletePatient(int id)
        {
            if (_patientRepository.Delete(id)==0)
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
