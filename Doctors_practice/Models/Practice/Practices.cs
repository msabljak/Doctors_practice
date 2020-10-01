using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace Doctors_practice.Models.Practice
{
    public class Practices
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public string Specialty { get; set; }
        public string Secret { get; set; }
    }

    public class CreatePractice : Practices
    {

    }

    public class ReadPractice : Practices
    {
        public ReadPractice(DataRow row)
        {
            if (row.Table.Columns.Contains("ID"))
            {
                ID = Convert.ToInt32(row["ID"]);
            }
            if (row.Table.Columns.Contains("Name"))
            {
                Name = row["Name"].ToString();
            }
            if (row.Table.Columns.Contains("Address"))
            {
                Address = row["Address"].ToString();
            }
            if (row.Table.Columns.Contains("Specialty"))
            {
                Specialty = row["Specialty"].ToString();
            }
            if (row.Table.Columns.Contains("Secret"))
            {
                Secret = row["Secret"].ToString();
            }
        }
    }
}
