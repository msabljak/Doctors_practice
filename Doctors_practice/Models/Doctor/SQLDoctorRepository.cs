using Doctors_practice.Models.Doctor;
using Doctors_practice.Models.Practice;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace Doctors_practice.Models.Patient
{
    public class SQLDoctorRepository : IDoctorRepository
    {
        SqlConnection _connection;
        string _connectionString;
        SqlDataAdapter _adapter;
        public SQLDoctorRepository()
        {
            _connectionString = "Data Source=db;Initial Catalog=Doctors_practice;Persist Security Info=True;User ID=SA;Password=<QWerT!13r4>";
        }
        public DoctorDTO Add(DoctorDTO doctorDTO)
        {
            using (_connection = new SqlConnection(_connectionString))
            {
                try
                {
                    SQLPracticeRepository practiceRepository = new SQLPracticeRepository();
                    if (practiceRepository.PracticeExists(doctorDTO.Practice_id))
                    {
                        var query = "insert into Doctor(Name,Surname,Practice_id) values (@name,@surname,@practice_id)";
                        SqlCommand sqlCommand = new SqlCommand(query, _connection);
                        sqlCommand.Parameters.AddWithValue("@name", doctorDTO.Name);
                        sqlCommand.Parameters.AddWithValue("@address", doctorDTO.Surname);
                        sqlCommand.Parameters.AddWithValue("@practice_id", doctorDTO.Practice_id);
                        _connection.Open();
                        sqlCommand.ExecuteNonQuery();
                        var doctor = new Doctors
                        {
                            Name = doctorDTO.Name,
                            Surname = doctorDTO.Surname,
                            Practice_id = doctorDTO.Practice_id
                        };
                        return DoctorToDTO(doctor);
                    }
                    else
                    {
                        return null;
                    }

                }
                catch
                {

                    throw;
                }
            }
        }

        public int Delete(int id)
        {            
            if (!DoctorExists(id))
            {
                return 0;
            }
            using (_connection = new SqlConnection(_connectionString))
            {
                try
                {
                    var query = $"delete from Doctor where id = {id}";
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

        public IEnumerable<DoctorDTO> GetAllDoctors()
        {
            using (_connection = new SqlConnection(_connectionString))
            {
                try
                {
                    _connection.Open();
                    DataTable _dt = new DataTable();
                    var query = "select ID, Name, Surname, Practice_id from Doctor";
                    _adapter = new SqlDataAdapter
                    {
                        SelectCommand = new SqlCommand(query, _connection)
                    };
                    _adapter.Fill(_dt);
                    List<DoctorDTO> doctorDTOs = new List<DoctorDTO>(_dt.Rows.Count);
                    if (_dt.Rows.Count > 0)
                    {
                        foreach (DataRow doctorRecord in _dt.Rows)
                        {
                            doctorDTOs.Add(DoctorToDTO(new ReadDoctor(doctorRecord)));
                        }
                    }
                    return doctorDTOs;
                }
                catch
                {

                    throw;
                }
            }                
        }

        public DoctorDTO GetDoctor(int id)
        {
            using (_connection = new SqlConnection(_connectionString))
            {
                try
                {
                    _connection.Open();
                    DataTable _dt = new DataTable();
                    var query = $"select ID, Name, Surname, Practice_id from Doctor where id={id}";
                    _adapter = new SqlDataAdapter
                    {
                        SelectCommand = new SqlCommand(query, _connection)
                    };
                    _adapter.Fill(_dt);
                    List<DoctorDTO> doctorDTOs = new List<DoctorDTO>(_dt.Rows.Count);
                    if (_dt.Rows.Count > 0)
                    {
                        foreach (DataRow doctorRecord in _dt.Rows)
                        {
                            doctorDTOs.Add(DoctorToDTO(new ReadDoctor(doctorRecord)));
                        }
                    }
                    return doctorDTOs[0];
                }
                catch
                {

                    throw;
                }
            }
        }

        public int Update(DoctorDTO doctorDTOChanges, int id)
        {
            if (!DoctorExists(id))
            {
                return 0;
            }
            using (_connection = new SqlConnection(_connectionString))
            {
                try
                {
                    SQLPracticeRepository practiceRepository = new SQLPracticeRepository();
                    if (practiceRepository.PracticeExists(doctorDTOChanges.Practice_id))
                    {
                        var query = $"update Doctor set Name=@name,Surname=@surname,Practice=@practice_id where id = {id}";
                        SqlCommand sqlCommand = new SqlCommand(query, _connection);
                        sqlCommand.Parameters.AddWithValue("Name", doctorDTOChanges.Name);
                        sqlCommand.Parameters.AddWithValue("Surname", doctorDTOChanges.Surname);
                        sqlCommand.Parameters.AddWithValue("Practice_id", doctorDTOChanges.Practice_id);
                        _connection.Open();
                        sqlCommand.ExecuteNonQuery();
                        return 1;
                    }
                    else
                    {
                        return -1;
                    }
                }
                catch
                {

                    throw;
                }
            }
        }
        public bool DoctorExists(int id)
        {
            using (_connection = new SqlConnection(_connectionString))
            {
                try
                {
                    var query = $"select ID from Doctor where id = {id}";
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
        private static DoctorDTO DoctorToDTO(Doctors doctor) =>
            new DoctorDTO
            {
                ID = doctor.ID,
                Name = doctor.Name,
                Surname = doctor.Surname,
                Practice_id = doctor.Practice_id
            };
    }
}
