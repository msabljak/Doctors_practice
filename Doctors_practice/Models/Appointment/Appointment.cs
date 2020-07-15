using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Doctors_practice.Models.Appointment
{
    public class Appointment
    {
        public int ID { get; set; }
        public int Patient_id { get; set; }
        public int Doctor_id { get; set; }
        public string Reason { get; set; }
        public string Secret { get; set; }
    }
}
