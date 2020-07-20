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
        public string Reason { get; set; }
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
            if (row.Table.Columns.Contains("Reason"))
            {
                Reason = row["Reason"].ToString();
            }
            if (row.Table.Columns.Contains("Secret"))
            {
                Secret = row["Secret"].ToString();
            }
        }
    }
}
