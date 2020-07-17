﻿using System;
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
            if (row.Table.Columns.Contains("Telephone"))
            {
                Telephone = row["Telephone"].ToString();
            }
            if (row.Table.Columns.Contains("Secret"))
            {
                Secret = row["Secret"].ToString();
            }    
        }
    }
}
