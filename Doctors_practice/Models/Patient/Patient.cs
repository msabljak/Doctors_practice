using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace Doctors_practice.Models
{
    public class Patient
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public string Telephone { get; set; }
        public string Secret { get; set; }
    }

    public class CreatePatient : Patient
    {

    }

    public class ReadPatient : Patient
    {
        public ReadPatient(DataRow row)
        {
            ID = Convert.ToInt32(row["ID"]);
            Name = row["Name"].ToString();
            Surname = row["Surname"].ToString();
            Telephone = row["Telephone"].ToString();
            Secret = row["Secret"].ToString();

        }
    }
}
