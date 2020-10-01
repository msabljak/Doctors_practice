using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Doctors_practice.Models
{
    public class HealthCard
    {
        public int ID { get; set; }
        public string HistoryOfIllness { get; set; }
        public string BloodType { get; set; }
        public string HereditaryIllnesses { get; set; }
    }
}
