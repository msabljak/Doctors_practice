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
using Polly;
using Microsoft.AspNetCore.Authorization;
using Doctors_practice.Services;
using Newtonsoft.Json;
using Microsoft.Extensions.Configuration;

namespace Doctors_practice.Controllers
{
    
    [ApiController]
    public class PatientsController : ControllerBase
    {
        private IPatientRepository _patientRepository;
        private IPatientClient _client;
        private IMediator _mediator;
        private ICacheService _cacheService;
        private IConfiguration _configuration;

        public PatientsController(IPatientRepository patientRepository, IPatientClient client, IMediator mediator, ICacheService cacheService, IConfiguration configuration)
        {
            _patientRepository = patientRepository;
            _client = client;
            _mediator = mediator;
            _cacheService = cacheService;
            _configuration = configuration;
        }
        // GET: Patients
        [Authorize(Policy = "Reader")]
        [HttpGet]
        [Route("Patients")]
        
        public async Task<IEnumerable<PatientDTO>> GetPatients()
        {
            if (_configuration.GetValue<string>("Properties:cacheEnabled") == "true")
            {
                if (await _cacheService.GetCacheValueAsync(Request.Path) == null)
                {
                    IEnumerable<PatientDTO> patientDTOs = _patientRepository.GetAllPatients();
                    string patientDTOsAsJSON = JsonConvert.SerializeObject(patientDTOs);
                    await _cacheService.SetCacheValueAsync(Request.Path, patientDTOsAsJSON);
                    return patientDTOs;
                }
                else
                {
                    return JsonConvert.DeserializeObject<IEnumerable<PatientDTO>>(await _cacheService.GetCacheValueAsync(Request.Path));
                }
            }
            return _patientRepository.GetAllPatients();
        }

        // GET: Patients/5
        [HttpGet]
        [Route("Patients/{id}")]
        public async Task<PatientDTO> GetPatient(int id)
        {
            if (_configuration.GetValue<string>("Properties:cacheEnabled") == "true")
            {
                if (await _cacheService.GetCacheValueAsync(Request.Path) == null)
                {
                    PatientDTO patientDTO = _patientRepository.GetPatients(id);
                    string patientDTOAsJSON = JsonConvert.SerializeObject(patientDTO);
                    await _cacheService.SetCacheValueAsync(Request.Path, patientDTOAsJSON);
                    return patientDTO;
                }
                else
                {
                    return JsonConvert.DeserializeObject<PatientDTO>(await _cacheService.GetCacheValueAsync(Request.Path));
                }
            }
            return _patientRepository.GetPatients(id);
        }

        // GET: Patients/FromPractices/5
        [HttpGet]
        [Route("Patients/FromPractices/{minimumDoctors}")]
        public string GetPatientsFromPractices(int minimumDoctors)
        {
            return _patientRepository.GetAllPatientsFromPracticesWithSpecificAmountOfDoctors(minimumDoctors);
        }

        [HttpGet]
        [Route("Patients/SlowRequest/{desiredAmount}")]
        public async Task<string> GetSlowRequest(int desiredAmount)
        {
            if (_configuration.GetValue<string>("Properties:cacheEnabled") == "true")
            {
                if (await _cacheService.GetCacheValueAsync(Request.Path) == null)
                {
                    string data = _patientRepository.SlowRequest(desiredAmount);
                    await _cacheService.SetCacheValueAsync(Request.Path, data);
                    return data;
                }
                else
                {
                    return await _cacheService.GetCacheValueAsync(Request.Path);
                }
            }
            return _patientRepository.SlowRequest(desiredAmount);
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

        ////POST: Patients/5/Charge
        //[HttpPost]
        //[Route("Patients/{id}/Charge")]
        //public async Task<ActionResult> PostPatientCharge (int id)
        //{
        //    var result = await _mediator.Send(new ChargePatientCommand(id));
        //    if (result == true)
        //    {
        //        return Ok();
        //    }
        //    return StatusCode(500);
        //}

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
