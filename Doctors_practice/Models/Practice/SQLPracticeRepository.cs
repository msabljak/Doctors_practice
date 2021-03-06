﻿using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace Doctors_practice.Models.Practice
{
    public class SQLPracticeRepository : IPracticeRepository
    {
        private SqlConnection _connection;
        private IConfiguration _configuration;
        private string _connectionString;
        private SqlDataAdapter _adapter;
        public SQLPracticeRepository(IConfiguration configuration)
        {
            _configuration = configuration;
            _connectionString = _configuration.GetConnectionString("sqlDB");
        }

        public PracticeDTO Add(PracticeDTO practiceDTO)
        {
            using(_connection = new SqlConnection(_connectionString))
            {
                try
                {
                    var query = "insert into Practice(Name,Address,Specialty) values (@name,@address,@specialty)";
                    SqlCommand sqlCommand = new SqlCommand(query, _connection);
                    sqlCommand.Parameters.AddWithValue("@name", practiceDTO.Name);
                    sqlCommand.Parameters.AddWithValue("@address", practiceDTO.Address);
                    sqlCommand.Parameters.AddWithValue("@specialty", practiceDTO.Specialty);
                    _connection.Open();
                    sqlCommand.ExecuteNonQuery();
                    var practice = new Practices
                    {
                        Name = practiceDTO.Name,
                        Address = practiceDTO.Address,
                        Specialty = practiceDTO.Specialty
                    };
                    return PracticeToDTO(practice);

                }
                catch
                {

                    throw;
                }
            }
        }

        public int Delete(int id)
        {
            if (!PracticeExists(id))
            {
                return 0;
            }
            using(_connection = new SqlConnection(_connectionString))
            {
                try
                {
                    var query = $"delete from Practice where id = {id}";
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

        public IEnumerable<PracticeDTO> GetAllPractices()
        {
            using (_connection=new SqlConnection(_connectionString))
            {
                try
                {
                    _connection.Open();
                    DataTable _dt = new DataTable();
                    var query = "select ID, Name, Address, Specialty from Practice";
                    _adapter = new SqlDataAdapter
                    {
                        SelectCommand = new SqlCommand(query,_connection)
                    };
                    _adapter.Fill(_dt);
                    List<PracticeDTO> practiceDTOs = new List<PracticeDTO>(_dt.Rows.Count);
                    if (_dt.Rows.Count>0)
                    {
                        foreach (DataRow practiceRecord in _dt.Rows)
                        {
                            practiceDTOs.Add(PracticeToDTO(new ReadPractice(practiceRecord)));
                        }
                    }
                    return practiceDTOs;
                }
                catch
                {

                    throw;
                }
            }
        }

        public PracticeDTO GetPractices(int id)
        {
            using (_connection = new SqlConnection(_connectionString))
            {
                try
                {
                    _connection.Open();
                    DataTable _dt = new DataTable();
                    var query = $"select ID, Name, Address, Specialty from Practice where id={id}";
                    _adapter = new SqlDataAdapter
                    {
                        SelectCommand = new SqlCommand(query, _connection)
                    };
                    _adapter.Fill(_dt);
                    List<PracticeDTO> practiceDTOs = new List<PracticeDTO>(_dt.Rows.Count);
                    if (_dt.Rows.Count > 0)
                    {
                        foreach (DataRow practiceRecord in _dt.Rows)
                        {
                            practiceDTOs.Add(PracticeToDTO(new ReadPractice(practiceRecord)));
                        }
                    }
                    return practiceDTOs[0];
                }
                catch
                {

                    throw;
                }
            }
        }

        public int Update(PracticeDTO practiceDTOChanges, int id)
        {
            if (!PracticeExists(id))
            {
                return 0;
            }
            using (_connection=new SqlConnection(_connectionString))
            {
                try
                {
                    var query = $"update Practice set Name=@name,Address=@address,Specialty=@specialty where id = {id}";
                    SqlCommand sqlCommand = new SqlCommand(query, _connection);
                    sqlCommand.Parameters.AddWithValue("Name", practiceDTOChanges.Name);
                    sqlCommand.Parameters.AddWithValue("Address", practiceDTOChanges.Address);
                    sqlCommand.Parameters.AddWithValue("Specialty", practiceDTOChanges.Specialty);
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

        public bool PracticeExists(int id)
        {
            using (_connection = new SqlConnection(_connectionString))
            {
                try
                {
                    var query = $"select ID from Practice where id = {id}";
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

        private static PracticeDTO PracticeToDTO(Practices practice) =>
            new PracticeDTO
            {
                ID = practice.ID,
                Name = practice.Name,
                Address = practice.Address,
                Specialty = practice.Specialty
            };
    }
}
