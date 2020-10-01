using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace Doctors_practice.Models.Appointment
{
    public class Appointments
    {
        public int ID { get; set; }
        public int Patient_id { get; set; }
        public int Doctor_id { get; set; }
        public string DiagnosisCode { get; set; }
        public string DiagnosisDescription { get; set; }
        public DateTime Date { get; set; }
        public string Secret { get; set; }
    }

    public class CreateAppointment : Appointments
    {

    }

    public class ReadAppointment : Appointments
    {
        public ReadAppointment(DataRow row)
        {
            if (row.Table.Columns.Contains("ID"))
            {
                ID = Convert.ToInt32(row["ID"]);
            }
            if (row.Table.Columns.Contains("Patient_id"))
            {
                Patient_id = Convert.ToInt32(row["Patient_id"]);
            }
            if (row.Table.Columns.Contains("Doctor_id"))
            {
                Doctor_id = Convert.ToInt32(row["Doctor_id"]);
            }
            if (row.Table.Columns.Contains("Diagnosis_code"))
            {
                DiagnosisCode = row["Diagnosis_code"].ToString();
            }
            if (row.Table.Columns.Contains("Diagnosis_description"))
            {
                DiagnosisDescription = row["Diagnosis_description"].ToString();
            }
            if (row.Table.Columns.Contains("Date"))
            {
                Date = Convert.ToDateTime(row["Date"]);
            }
            if (row.Table.Columns.Contains("Secret"))
            {
                Secret = row["Secret"].ToString();
            }
        }
    }
}
