using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace MyWeb.Models
{
    public class CompanyContext : DbContext
    {
        public DbSet<Emp> Emps { get; set; }
        public DbSet<Company> Companies { get; set; }

        public CompanyContext(DbContextOptions<CompanyContext> options) : base (options)
        {
            Database.EnsureCreated();
        }
    }
}
