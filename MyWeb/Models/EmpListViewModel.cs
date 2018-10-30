using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyWeb.Models
{
    public class EmpListViewModel
    {
        public IEnumerable<Emp> Emps { get; set; }
        public SelectList Companies { get; set; }
        public string Name { get; set; }
    }
}
