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

namespace Doctors_practice.Controllers
{
    
    [ApiController]
    public class PatientsController : ControllerBase
    {
        private SqlConnection _connection;
        private string _connectionString = "Data Source=localhost;Initial Catalog=Doctors_practice;Persist Security Info=True;User ID=SA;Password=<QWerT!13r4>";
        private SqlDataAdapter _adapter;

        // GET: Patients
        [HttpGet]
        [Route("Patients")]
        
        public IEnumerable<PatientDTO> GetPatients()
        {
            using(_connection=new SqlConnection(_connectionString))
            {
                try
                {
                    _connection.Open();
                    DataTable _dt = new DataTable();
                    var query = "Select * from Patient";
                    _adapter = new SqlDataAdapter
                    {
                        SelectCommand = new SqlCommand(query, _connection)
                    };
                    _adapter.Fill(_dt);
                    List<PatientDTO> patientDTOs = new List<PatientDTO>(_dt.Rows.Count);
                    if (_dt.Rows.Count > 0)
                    {
                        foreach (DataRow patientRecord in _dt.Rows)
                        {
                            patientDTOs.Add(PatientToDTO(new ReadPatient(patientRecord)));
                        }
                    }
                    return patientDTOs;
                }
                catch
                {
                    throw;
                }
            }            
        }

        // GET: Patients/5
        [HttpGet]
        [Route("Patients/{id}")]
        public IEnumerable<PatientDTO> GetPatient(int id)
        {
            using (_connection = new SqlConnection(_connectionString))
            {
                try
                {
                    _connection.Open();
                    DataTable _dt = new DataTable();
                    var query = $"Select * from Patient where id={id}";
                    _adapter = new SqlDataAdapter
                    {
                        SelectCommand = new SqlCommand(query, _connection)
                    };
                    _adapter.Fill(_dt);
                    List<PatientDTO> patientDTOs = new List<PatientDTO>(_dt.Rows.Count);
                    if (_dt.Rows.Count > 0)
                    {
                        foreach (DataRow patientRecord in _dt.Rows)
                        {
                            patientDTOs.Add(PatientToDTO(new ReadPatient(patientRecord)));
                        }
                    }
                    return patientDTOs;
                }
                catch
                {
                    throw;
                }
            }
        }

        // PUT: Patients/5
        [HttpPut]
        [Route("Patients/{id}")]
        public async Task<IActionResult> PutPatient(int id, PatientDTO patientDTO)
        {
            //if (id != patientDTO.ID)
            //{
            //    return BadRequest();
            //}

            //var patient = await _context.Patients.FindAsync(id);
            //if(patient == null)
            //{
                return NotFound();
            //}

            //patient.Name = patientDTO.Name;
            //patient.Surname = patientDTO.Surname;
            //patient.Telephone = patientDTO.Telephone;

            //try
            //{
            //   // await _context.SaveChangesAsync();
            //}
            //catch (DbUpdateConcurrencyException)
            //{
            //    if (!PatientExists(id))
            //    {
            //        return NotFound();
            //    }
            //    else
            //    {
            //        throw;
            //    }
            //}

            //return NoContent();
        }

        // POST: Patients
        [HttpPost]
        [Route("Patients")]
        public async Task<ActionResult<PatientDTO>> PostPatient(PatientDTO patientDTO)
        {
            var patient = new Patient
            {
                Name = patientDTO.Name,
                Surname = patientDTO.Surname,
                Telephone = patientDTO.Telephone
            };

            //_context.Patients.Add(patient);
            //await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetPatient), new { id = patient.ID }, PatientToDTO(patient));
        }

        // DELETE: Patients/5
        [HttpDelete("{id}")]
        [Route("Patients/{id}")]
        public async Task<ActionResult<Patient>> DeletePatient(int id)
        {
           // var patient = await _context.Patients.FindAsync(id);
            //if (patient == null)
            //{
                return NotFound();
            //}

            //_context.Patients.Remove(patient);
            //await _context.SaveChangesAsync();

            //return patient;
        }

        private bool PatientExists(int id)
        {
            //return _context.Patients.Any(e => e.ID == id);
            return true;
        }

        private static PatientDTO PatientToDTO(Patient patient) =>
            new PatientDTO
            {
                ID = patient.ID,
                Name = patient.Name,
                Surname = patient.Surname,
                Telephone = patient.Telephone
            };
    }
}
