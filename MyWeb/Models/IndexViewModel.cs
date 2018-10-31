using System.Collections.Generic;

namespace MyWeb.Models
{
    public class IndexViewModel
    {
        public IEnumerable<Emp> Emps { get; set; }
        public PageViewModel PageViewModel { get; set; }
        public FilterViewModel FilterViewModel { get; set; }
        public SortViewModel SortViewModel { get; set; }
    }
}
