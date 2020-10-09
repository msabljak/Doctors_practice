using Apache.NMS;
using Apache.NMS.ActiveMQ;
using Microsoft.AspNetCore.Razor.Language.Extensions;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace Doctors_practice.Models.Patient
{
    public class SQLPatientRepository : IPatientRepository
    {
        private SqlConnection _connection;
        private IConfiguration _configuration;
        private string _connectionString;
        private SqlDataAdapter _adapter;
        public SQLPatientRepository(IConfiguration configuration)
        {
            _configuration = configuration;
            _connectionString = _configuration.GetConnectionString("sqlDB");
            //_connectionString = "Data Source=localhost;Initial Catalog=Doctors_practice;User ID=SA;Password=<QWerT!13r4>";
        }
        public PatientDTO Add(PatientDTO patientDTO)
        {
            using (_connection = new SqlConnection(_connectionString))
            {
                try
                {
                    var query = "insert into Patient (Name,Surname,Birthdate,Gender,Email,Health_card_id,Secret) values (@name,@surname,@birthdate,@gender,@email,@health_card_id,@secret)";
                    SqlCommand sqlCommand = new SqlCommand(query, _connection);
                    sqlCommand.Parameters.AddWithValue("@name", patientDTO.Name);
                    sqlCommand.Parameters.AddWithValue("@surname", patientDTO.Surname);
                    sqlCommand.Parameters.AddWithValue("@birthdate", patientDTO.Birthdate);
                    sqlCommand.Parameters.AddWithValue("@gender", patientDTO.Gender);
                    sqlCommand.Parameters.AddWithValue("@email", patientDTO.Email);
                    sqlCommand.Parameters.AddWithValue("@health_card_id", patientDTO.HealthCard_id);
                    sqlCommand.Parameters.AddWithValue("@secret", "-");
                    _connection.Open();
                    sqlCommand.ExecuteNonQuery();
                    var patient = new Patients
                    {
                        Name = patientDTO.Name,
                        Surname = patientDTO.Surname,
                        Birthdate = patientDTO.Birthdate,
                        Gender = patientDTO.Gender,
                        Email = patientDTO.Email,
                        HealthCard_id = patientDTO.HealthCard_id
                    };
                    return PatientToDTO(patient);
                }
                catch
                {
                    throw;
                }
            }
        }

        public Task<PatientDTO> AddAsync(PatientDTO patientDTO)
        {
            using (_connection = new SqlConnection(_connectionString))
            {
                try
                {
                    var query = "insert into Patient (Name,Surname,Birthdate,Gender,Email,Health_card_id,Secret) values (@name,@surname,@birthdate,@gender,@email,@health_card_id,@secret);SELECT CAST(scope_identity() AS int)";
                    SqlCommand sqlCommand = new SqlCommand(query, _connection);
                    sqlCommand.Parameters.AddWithValue("@name", patientDTO.Name);
                    sqlCommand.Parameters.AddWithValue("@surname", patientDTO.Surname);
                    sqlCommand.Parameters.AddWithValue("@birthdate", patientDTO.Birthdate);
                    sqlCommand.Parameters.AddWithValue("@gender", patientDTO.Gender);
                    sqlCommand.Parameters.AddWithValue("@email", patientDTO.Email);
                    sqlCommand.Parameters.AddWithValue("@health_card_id", patientDTO.HealthCard_id);
                    sqlCommand.Parameters.AddWithValue("@secret", "-");
                    _connection.Open();                    
                    var newID = (int)sqlCommand.ExecuteScalar();
                    var patient = new Patients
                    {
                        ID = newID,
                        Name = patientDTO.Name,
                        Surname = patientDTO.Surname,
                        Birthdate = patientDTO.Birthdate,
                        Gender = patientDTO.Gender,
                        Email = patientDTO.Email,
                        HealthCard_id = patientDTO.HealthCard_id
                    };
                    return Task.FromResult(PatientToDTO(patient));
                }
                catch
                {
                    throw;
                }
            }
        }
        public int Delete(int id)
        {
            if (!PatientExists(id))
            {
                return 0;
            }
            using (_connection = new SqlConnection(_connectionString))
            {
                try
                {
                    var query = $"delete from Patient where id = {id}";
                    SqlCommand sqlCommand = new SqlCommand(query, _connection);
                    _connection.Open();
                     sqlCommand.ExecuteNonQuery();
                     return 1;
                }
                catch
                {

                    throw;
                }
            }
        }
        public Task<int> DeleteAsync(int id)
        {
            if (!PatientExists(id))
            {
                return Task.FromResult(0);
            }
            using (_connection = new SqlConnection(_connectionString))
            {
                try
                {
                    var query = $"delete from Patient where id = {id}";
                    SqlCommand sqlCommand = new SqlCommand(query, _connection);
                    _connection.Open();
                    sqlCommand.ExecuteNonQuery();
                    return Task.FromResult(1);
                }
                catch
                {

                    throw;
                }
            }
        }

        public IEnumerable<PatientDTO> GetAllPatients()
        {
            using (_connection = new SqlConnection(_connectionString))
            {
                try
                {
                    _connection.Open();
                    DataTable _dt = new DataTable();
                    var query = "Select ID, Name, Surname, Birthdate, Gender, Email, Health_card_id, Secret from Patient";
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

        public PatientDTO GetPatients(int id)
        {
            using (_connection = new SqlConnection(_connectionString))
            {
                try
                {
                    _connection.Open();
                    DataTable _dt = new DataTable();
                    var query = $"Select ID, Name, Surname, Birthdate, Gender, Email, Health_card_id, Secret from Patient where id={id}";
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
                    return patientDTOs[0];
                }
                catch
                {
                    throw;
                }
            }
        }

        public string GetAllPatientsFromPracticesWithSpecificAmountOfDoctors(int requiredNumber)
        {
            using (_connection = new SqlConnection(_connectionString))
            {
                try
                {
                    _connection.Open();
                    DataTable _dt = new DataTable();
                    var query = $"SELECT pr.id, pr.Name as Practice_name, p.Name as Patient_name, p.Surname as Patient_surname, a.Diagnosis_description, a.Date FROM Patient p " +
                        $"JOIN Appointment a ON p.ID = a.Patient_id " +
                        $"JOIN Doctor d ON a.Doctor_id = d.ID " +
                        $"JOIN Practice pr ON d.Practice_id = pr.ID " +
                        $"WHERE pr.ID IN(" +
                        $"SELECT pr.ID as Practice_doctors FROM Practice pr " +
                        $"JOIN Doctor d ON pr.ID = d.Practice_id " +
                        $"GROUP BY pr.ID " +
                        $"HAVING COUNT(pr.ID) > {requiredNumber}) " +
                        $"AND p.Name LIKE 'a__%' " +
                        $"GROUP BY pr.ID,a.Date,p.Surname,p.Name, pr.Name, a.Diagnosis_description " +
                        $"ORDER BY pr.ID; ";
                    _adapter = new SqlDataAdapter
                    {
                        SelectCommand = new SqlCommand(query, _connection)
                    };
                    _adapter.Fill(_dt);
                    string data = "";
                    if (_dt.Rows.Count > 0)
                    {
                        foreach (DataRow row in _dt.Rows)
                        {
                            foreach (object item in row.ItemArray)
                            {
                                data = data + item.ToString() + " ";
                            }
                            data = data + "\n";
                        }
                    }
                    return data;
                }
                catch
                {

                    throw;
                }
            }
        }

        public string SlowRequest(int desiredAmount)
        {
            using (_connection = new SqlConnection(_connectionString))
            {
                try
                {
                    string data = "";
                    _connection.Open();
                    DataTable _dt = new DataTable();
                    var query = "SELECT Patient.Name, Doctor.Surname FROM Patient, Doctor";
                    _adapter = new SqlDataAdapter
                    {
                        SelectCommand = new SqlCommand(query, _connection)
                    };
                    _adapter.Fill(_dt);
                    int counter = 0;
                    if (_dt.Rows.Count > 0)
                    {
                        foreach (DataRow row in _dt.Rows)
                        {
                            foreach (object item in row.ItemArray)
                            {
                                data = data + item.ToString() + " ";
                            }
                            data = data + "\n";
                            counter++;
                            if (counter == desiredAmount)
                            {
                                break;
                            }
                        }
                    }
                    return data;
                }
                catch
                {

                    throw;
                }
            }
        }

        public int Update(PatientDTO patientDTOChanges, int id)
        {
            if (!PatientExists(id))
            {
                return 0;
            }
            using (_connection = new SqlConnection(_connectionString))
            {
                try
                {                    
                    var query = $"update Patient set Name=@name,Surname=@surname,Birthdate=@birthdate,Gender=@gender,Email=@email,Health_card_id=@health_card_id,Secret=@secret where id = {id}";
                    SqlCommand sqlCommand = new SqlCommand(query, _connection);
                    sqlCommand.Parameters.AddWithValue("@name", patientDTOChanges.Name);
                    sqlCommand.Parameters.AddWithValue("@surname", patientDTOChanges.Surname);
                    sqlCommand.Parameters.AddWithValue("@birthdate", patientDTOChanges.Birthdate);
                    sqlCommand.Parameters.AddWithValue("@gender",patientDTOChanges.Gender);
                    sqlCommand.Parameters.AddWithValue("@email", patientDTOChanges.Email);
                    sqlCommand.Parameters.AddWithValue("@health_card_id", patientDTOChanges.HealthCard_id);
                    sqlCommand.Parameters.AddWithValue("@secret", "-");
                    _connection.Open();
                    sqlCommand.ExecuteNonQuery();
                    return 1;                                        
                }
                catch
                {
                    throw;
                }
            }
        }

        public bool PatientExists(int id)
        {
            using (_connection = new SqlConnection(_connectionString))
            {
                try
                {
                    var query = $"select ID from Patient where id = {id}";
                    SqlCommand sqlCommand = new SqlCommand(query, _connection);
                    _connection.Open();
                    SqlDataReader reader = sqlCommand.ExecuteReader();
                    if (reader.HasRows)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                catch
                {

                    throw;
                }
            }
        }

        private static PatientDTO PatientToDTO(Patients patient) =>
            new PatientDTO
            {
                ID = patient.ID,
                Name = patient.Name,
                Surname = patient.Surname,
                Birthdate = patient.Birthdate,
                Gender = patient.Gender,
                Email = patient.Email,
                HealthCard_id = patient.HealthCard_id
            };
    }
}
