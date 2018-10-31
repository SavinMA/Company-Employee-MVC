using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MyWeb.Models;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace MyWeb.Models
{
    public class FilterViewModel
    {
        public SelectList Companies { get; private set; }
        public int? SelectedCompany { get; private set; }
        public string SelectedName { get; private set; }

        public FilterViewModel(List<Company> companies, int? company, string name)
        {
            companies.Insert(0, new Company { Name = "Все", Id = 0 });
            Companies = new SelectList(companies, "Id", "Name", company);
            SelectedCompany = company;
            SelectedName = name;
        }
    }
}
