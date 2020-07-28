using Microsoft.Data.SqlClient;
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
        private string _connectionString;
        private SqlDataAdapter _adapter;
        public SQLPatientRepository()
        {
            _connectionString = "Data Source=db;Initial Catalog=Doctors_practice;User ID=SA;Password=<QWerT!13r4>";
            //_connectionString = "Data Source=localhost;Initial Catalog=Doctors_practice;User ID=SA;Password=<QWerT!13r4>";
        }
        public PatientDTO Add(PatientDTO patientDTO)
        {
            using (_connection = new SqlConnection(_connectionString))
            {
                try
                {
                    var query = "insert into Patient (Name,Surname,Telephone,Secret) values (@name,@surname,@telephone,@secret)";
                    SqlCommand sqlCommand = new SqlCommand(query, _connection);
                    sqlCommand.Parameters.AddWithValue("@name", patientDTO.Name);
                    sqlCommand.Parameters.AddWithValue("@surname", patientDTO.Surname);
                    sqlCommand.Parameters.AddWithValue("@telephone", patientDTO.Telephone);
                    sqlCommand.Parameters.AddWithValue("@secret", "-");
                    _connection.Open();
                    sqlCommand.ExecuteNonQuery();
                    var patient = new Patients
                    {
                        Name = patientDTO.Name,
                        Surname = patientDTO.Surname,
                        Telephone = patientDTO.Telephone
                    };
                    return PatientToDTO(patient);
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

        public IEnumerable<PatientDTO> GetAllPatients()
        {
            using (_connection = new SqlConnection(_connectionString))
            {
                try
                {
                    _connection.Open();
                    DataTable _dt = new DataTable();
                    var query = "Select ID, Name, Surname, Telephone, Secret from Patient";
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
                    var query = $"Select ID, Name, Surname, Telephone, Secret from Patient where id={id}";
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
                    var query = $"update Patient set Name=@name,Surname=@surname,Telephone=@telephone,Secret=@secret where id = {id}";
                    SqlCommand sqlCommand = new SqlCommand(query, _connection);
                    sqlCommand.Parameters.AddWithValue("@name", patientDTOChanges.Name);
                    sqlCommand.Parameters.AddWithValue("@surname", patientDTOChanges.Surname);
                    sqlCommand.Parameters.AddWithValue("@telephone", patientDTOChanges.Telephone);
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
                Telephone = patient.Telephone
            };
    }
}
