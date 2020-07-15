using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Doctors_practice.Models.Practice
{
    public class PracticeContext : DbContext
    {
        public PracticeContext(DbContextOptions<PracticeContext> options)
            : base(options)
        {

        }

        public DbSet<Practice> Practices { get; set; }
    }
}
