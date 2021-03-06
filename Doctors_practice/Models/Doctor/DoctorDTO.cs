﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Doctors_practice.Models.Doctor
{
    public class DoctorDTO
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public DateTime Birthdate { get; set; }
        public string Telephone { get; set; }
        public string Email { get; set; }
        public int Practice_id { get; set; }
    }
}
