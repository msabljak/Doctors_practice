using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace Doctors_practice.Models.Doctor
{
    public class Doctors
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public DateTime Birthdate { get; set; }
        public string Telephone { get; set; }
        public string Email { get; set; }
        public int Practice_id { get; set; }
        public string Secret { get; set; }
    }

    public class CreateDoctor : Doctors
    {

    }

    public class ReadDoctor : Doctors
    {
        public ReadDoctor(DataRow row)
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
            if (row.Table.Columns.Contains("Birthdate"))
            {
                Birthdate = Convert.ToDateTime(row["Birthdate"]);
            }
            if (row.Table.Columns.Contains("Telephone"))
            {
                Surname = row["Telephone"].ToString();
            }
            if (row.Table.Columns.Contains("Email"))
            {
                Surname = row["Email"].ToString();
            }
            if (row.Table.Columns.Contains("Practice_id"))
            {
                Practice_id = Convert.ToInt32(row["Practice_id"]);
            }
            if (row.Table.Columns.Contains("Secret"))
            {
                Secret = row["Secret"].ToString();
            }
        }
    }
}
