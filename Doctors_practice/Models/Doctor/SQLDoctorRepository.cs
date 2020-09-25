﻿using Doctors_practice.Models.Doctor;
using Doctors_practice.Models.Practice;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace Doctors_practice.Models.Patient
{
    public class SQLDoctorRepository : IDoctorRepository
    {
        private SqlConnection _connection;
        private IConfiguration _configuration;
        private string _connectionString;
        private SqlDataAdapter _adapter;
        public SQLDoctorRepository(IConfiguration configuration)
        {
            _configuration = configuration;
            _connectionString = _configuration.GetConnectionString("sqlDB");
        }
        public DoctorDTO Add(DoctorDTO doctorDTO)
        {
            using (_connection = new SqlConnection(_connectionString))
            {
                try
                {
                    SQLPracticeRepository practiceRepository = new SQLPracticeRepository(_configuration);
                    if (practiceRepository.PracticeExists(doctorDTO.Practice_id))
                    {
                        var query = "insert into Doctor(Name,Surname,Birthdate,Practice_id) values (@name,@surname,@birthdate,@practice_id)";
                        SqlCommand sqlCommand = new SqlCommand(query, _connection);
                        sqlCommand.Parameters.AddWithValue("@name", doctorDTO.Name);
                        sqlCommand.Parameters.AddWithValue("@surname", doctorDTO.Surname);
                        sqlCommand.Parameters.AddWithValue("@birthdate", doctorDTO.Birthdate);
                        sqlCommand.Parameters.AddWithValue("@practice_id", doctorDTO.Practice_id);
                        _connection.Open();
                        sqlCommand.ExecuteNonQuery();
                        var doctor = new Doctors
                        {
                            Name = doctorDTO.Name,
                            Surname = doctorDTO.Surname,
                            Birthdate = doctorDTO.Birthdate,
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
                    var query = "select ID, Name, Surname, Birthdate, Practice_id from Doctor";
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
                    var query = $"select ID, Name, Surname, Birthdate, Practice_id from Doctor where id={id}";
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
                    SQLPracticeRepository practiceRepository = new SQLPracticeRepository(_configuration);
                    if (practiceRepository.PracticeExists(doctorDTOChanges.Practice_id))
                    {
                        var query = $"update Doctor set Name=@name,Surname=@surname,Birthdate=@birthdate,Practice=@practice_id where id = {id}";
                        SqlCommand sqlCommand = new SqlCommand(query, _connection);
                        sqlCommand.Parameters.AddWithValue("Name", doctorDTOChanges.Name);
                        sqlCommand.Parameters.AddWithValue("Surname", doctorDTOChanges.Surname);
                        sqlCommand.Parameters.AddWithValue("Birthdate", doctorDTOChanges.Birthdate);
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
                Birthdate = doctor.Birthdate,
                Practice_id = doctor.Practice_id
            };
    }
}
