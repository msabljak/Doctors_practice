using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace Doctors_practice.Models
{
    public class Patients
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public DateTime Birthdate { get; set; }
        public char Gender { get; set; }
        public string Email { get; set; }
        public int HealthCard_id { get; set; }
        public string Secret { get; set; }
    }

    public class CreatePatient : Patients
    {

    }

    public class ReadPatient : Patients
    {
        public ReadPatient(DataRow row)
        {
            if (row.Table.Columns.Contains("ID"))
            {
                ID = Convert.ToInt32(row["ID"]);
            }
            if (row.Table.Columns.Contains("Name"))
            {
                Name = row["Name"].ToString();
            }
            if (row.Table.Columns.Contains("Surname"))
            {
                Surname = row["Surname"].ToString();
            }
            if (row.Table.Columns.Contains("Birthdate") && row["Birthdate"].ToString() != "")
            {
                Birthdate = Convert.ToDateTime(row["Birthdate"]);
            }
            if (row.Table.Columns.Contains("Gender"))
            {
                if (row["Gender"].ToString() == "")
                {
                    Gender = '\0';
                }
                else
                {
                    Gender = row["Gender"].ToString().ToCharArray()[0];
                }                
            }
            if (row.Table.Columns.Contains("Health_card_id"))
            {
                if (row["Health_card_id"].ToString() == "")
                {
                    HealthCard_id = -1;
                }
                else
                {
                    HealthCard_id = Convert.ToInt32(row["Health_card_id"]);
                }                
            }
            if (row.Table.Columns.Contains("Secret"))
            {
                Secret = row["Secret"].ToString();
            }    
        }
    }
}
