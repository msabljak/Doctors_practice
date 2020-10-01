using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Doctors_practice.Models
{
    public class PatientDTO
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public DateTime Birthdate { get; set; }
        public char Gender { get; set; }
        public string Email { get; set; }
        public int HealthCard_id { get; set; }
    }
}
