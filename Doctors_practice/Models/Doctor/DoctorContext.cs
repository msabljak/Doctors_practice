using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Doctors_practice.Models.Doctor
{
    public class DoctorContext : DbContext
    {
        public DoctorContext(DbContextOptions<DoctorContext> options)
            : base(options)
        {

        }

        public DbSet<Doctor> Doctors { get; set; }
    }
}
