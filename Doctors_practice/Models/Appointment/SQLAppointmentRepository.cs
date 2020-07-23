using Doctors_practice.Models.Appointment;
using Doctors_practice.Models.Patient;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace Doctors_practice.Models.Doctor
{
    public class SQLAppointmentRepository : IAppointmentRepository
    {
        SqlConnection _connection;
        string _connectionString;
        SqlDataAdapter _adapter;

        public SQLAppointmentRepository()
        {
            _connectionString = "Data Source=db;Initial Catalog=Doctors_practice;Persist Security Info=True;User ID=SA;Password=<QWerT!13r4>";
        }
        public AppointmentDTO Add(AppointmentDTO appointmentDTO)
        {
            using (_connection = new SqlConnection(_connectionString))
            {
                try
                {
                    SQLPatientRepository patientRepository = new SQLPatientRepository();
                    SQLDoctorRepository doctorRepository = new SQLDoctorRepository();
                    if (patientRepository.PatientExists(appointmentDTO.Patient_id)&& doctorRepository.DoctorExists(appointmentDTO.Doctor_id))
                    {
                        var query = "insert into Appointment(Patient_id,Doctor_id,Reason) values (@patient_id,@doctor_id,@reason)";
                        SqlCommand sqlCommand = new SqlCommand(query, _connection);
                        sqlCommand.Parameters.AddWithValue("@patient_id", appointmentDTO.Patient_id);
                        sqlCommand.Parameters.AddWithValue("@doctor_id", appointmentDTO.Doctor_id);
                        sqlCommand.Parameters.AddWithValue("@reason", appointmentDTO.Reason);
                        _connection.Open();
                        sqlCommand.ExecuteNonQuery();
                        var appointment = new Appointments
                        {
                            Patient_id = appointmentDTO.Patient_id,
                            Doctor_id = appointmentDTO.Doctor_id,
                            Reason = appointmentDTO.Reason
                        };
                        return AppointmentToDTO(appointment);
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
            if (!AppointmentExists(id))
            {
                return 0;
            }
            using (_connection = new SqlConnection(_connectionString))
            {
                try
                {
                    var query = $"delete from Appointment where id = {id}";
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

        public IEnumerable<AppointmentDTO> GetAllAppointments()
        {
            using (_connection = new SqlConnection(_connectionString))
            {
                try
                {
                    _connection.Open();
                    DataTable _dt = new DataTable();
                    var query = $"select ID, Patient_id, Doctor_id, Reason from Appointment";
                    _adapter = new SqlDataAdapter
                    {
                        SelectCommand = new SqlCommand(query, _connection)
                    };
                    _adapter.Fill(_dt);
                    List<AppointmentDTO> appointmentDTOs = new List<AppointmentDTO>(_dt.Rows.Count);
                    if (_dt.Rows.Count > 0)
                    {
                        foreach (DataRow appointmentRecord in _dt.Rows)
                        {
                            appointmentDTOs.Add(AppointmentToDTO(new ReadAppointment(appointmentRecord)));
                        }
                    }
                    return appointmentDTOs;
                }
                catch
                {

                    throw;
                }
            }
        }

        public AppointmentDTO GetAppointment(int id)
        {
            using (_connection = new SqlConnection(_connectionString))
            {
                try
                {
                    _connection.Open();
                    DataTable _dt = new DataTable();
                    var query = $"select ID, Patient_id, Doctor_id, Reason from Appointment where id = {id}";
                    _adapter = new SqlDataAdapter
                    {
                        SelectCommand = new SqlCommand(query, _connection)
                    };
                    _adapter.Fill(_dt);
                    List<AppointmentDTO> appointmentDTOs = new List<AppointmentDTO>(_dt.Rows.Count);
                    if (_dt.Rows.Count > 0)
                    {
                        foreach (DataRow appointmentRecord in _dt.Rows)
                        {
                            appointmentDTOs.Add(AppointmentToDTO(new ReadAppointment(appointmentRecord)));
                        }
                    }
                    return appointmentDTOs[0];
                }
                catch
                {

                    throw;
                }
            }
        }

        public int Update(AppointmentDTO appointmentDTOChanges, int id)
        {
            if (!AppointmentExists(id))
            {
                return 0;
            }
            using (_connection = new SqlConnection(_connectionString))
            {
                try
                {
                    SQLPatientRepository patientRepository = new SQLPatientRepository();
                    SQLDoctorRepository doctorRepository = new SQLDoctorRepository();
                    if (patientRepository.PatientExists(appointmentDTOChanges.Patient_id) && doctorRepository.DoctorExists(appointmentDTOChanges.Doctor_id))
                    {
                        var query = $"update Appointment set Patient_id=@patient_id,Doctor_id=@doctor_id,Reason=@reason where id = {id}";
                        SqlCommand sqlCommand = new SqlCommand(query, _connection);
                        sqlCommand.Parameters.AddWithValue("@patient_id", appointmentDTOChanges.Patient_id);
                        sqlCommand.Parameters.AddWithValue("@doctor_id", appointmentDTOChanges.Doctor_id);
                        sqlCommand.Parameters.AddWithValue("@reason", appointmentDTOChanges.Reason);
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

        private bool AppointmentExists(int id)
        {
            using (_connection = new SqlConnection(_connectionString))
            {
                try
                {
                    var query = $"select ID from Appointment where id = {id}";
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
        private static AppointmentDTO AppointmentToDTO(Appointments appointment) =>
            new AppointmentDTO
            {
                ID = appointment.ID,
                Doctor_id = appointment.Doctor_id,
                Patient_id = appointment.Patient_id,
                Reason = appointment.Reason
            };
    }
}
