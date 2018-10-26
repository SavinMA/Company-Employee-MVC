using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyWeb.Models
{
    public class Company
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }

    public class Emp
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Age { get; set; }
        public int CompanyID { get; set; }
        public Company Company { get; set; }
    }
}
