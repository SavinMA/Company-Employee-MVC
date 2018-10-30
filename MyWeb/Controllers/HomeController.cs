using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MyWeb.Models;

namespace MyWeb.Controllers
{
    public class HomeController : Controller
    {
        private readonly CompanyContext db;

        public HomeController(CompanyContext context)
        {
            db = context;
        }

        [HttpGet]
        public ActionResult Index(int? company, string name, SortState sortOrder = SortState.NameAsc)
        {
            IQueryable<Emp> emps = db.Emps.Include(p => p.Company);
            if (company != null && company != 0)
                emps = emps.Where(p => p.CompanyID == company);
            if (!string.IsNullOrEmpty(name))
                emps = emps.Where(p => p.Name.Contains(name));

            ViewData["NameSort"] = sortOrder == SortState.NameAsc ? SortState.NameDesc : SortState.NameAsc;
            ViewData["AgeSort"] = sortOrder == SortState.AgeAsc ? SortState.AgeDesc : SortState.AgeAsc;
            ViewData["CompSort"] = sortOrder == SortState.CompanyAsc ? SortState.CompanyDesc : SortState.CompanyAsc;

            switch (sortOrder)
            {
                case SortState.NameDesc:
                    emps = emps.OrderByDescending(s => s.Name);
                    break;
                case SortState.AgeAsc:
                    emps = emps.OrderBy(s => s.Age);
                    break;
                case SortState.AgeDesc:
                    emps = emps.OrderByDescending(s => s.Age);
                    break;
                case SortState.CompanyAsc:
                    emps = emps.OrderBy(s => s.Company.Name);
                    break;
                case SortState.CompanyDesc:
                    emps = emps.OrderByDescending(s => s.Company.Name);
                    break;
                default:
                    emps = emps.OrderBy(s => s.Name);
                    break;
            }
            ViewData["Message"] = "Сотрудники";

            List<Company> companies = db.Companies.ToList();
            //            List<Company> companies = db.Companies.Include(p=> p.Id == company).ToList();

            companies.Insert(0, new Company { Name = "Все", Id = 0 });

            EmpListViewModel viewModel = new EmpListViewModel
            {
                Emps = emps.ToList(),
                Companies = new SelectList(companies, "Id", "Name"),
                Name = name
            };

            return View(viewModel);
        }
/*
        [HttpGet]
        public IActionResult Sort(SortState sortOrder = SortState.NameAsc)
        {
            IQueryable<Emp> emps = emp.AsQueryable();

            ViewData["NameSort"] = sortOrder == SortState.NameAsc ? SortState.NameDesc : SortState.NameAsc;
            ViewData["AgeSort"] = sortOrder == SortState.AgeAsc ? SortState.AgeDesc : SortState.AgeAsc;
            ViewData["CompSort"] = sortOrder == SortState.CompanyAsc ? SortState.CompanyDesc : SortState.CompanyAsc;
            
            switch (sortOrder)
            {
                case SortState.NameDesc:
                    emps = emps.OrderByDescending(s => s.Name);
                    break;
                case SortState.AgeAsc:
                    emps = emps.OrderBy(s => s.Age);
                    break;
                case SortState.AgeDesc:
                    emps = emps.OrderByDescending(s => s.Age);
                    break;
                case SortState.CompanyAsc:
                    emps = emps.OrderBy(s => s.Company.Name);
                    break;
                case SortState.CompanyDesc:
                    emps = emps.OrderByDescending(s => s.Company.Name);
                    break;
                default:
                    emps = emps.OrderBy(s => s.Name);
                    break;
            }
            

            List<Company> companies = db.Companies.ToList();
            companies.Insert(0, new Company { Name = "Все", Id = 0 });

            EmpListViewModel viewModel = new EmpListViewModel
            {
                Companies = new SelectList(db.Companies.ToList(), "Id", "Name"),
                Emps = emps.ToList(),
                Name = "Все"
            };
            
            return View("Index", viewModel);
        }
        */
        public IActionResult Company()
        {
            ViewData["Message"] = "Компании";
            return View(db.Companies);
        }

        public IActionResult CreateEmp()
        {
            ViewBag.companies = new SelectList(db.Companies, "Id", "Name");
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateEmp(Emp emp)
        {
            Company company = db.Companies.FirstOrDefault(c => c.Id == emp.CompanyID);
            db.Emps.Add(emp);
            await db.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id != null)
            {
                Emp emp = await db.Emps.FirstOrDefaultAsync(p => p.Id == id);
                if (emp != null)
                {
                    ViewData["Title"] = "Редактирование сотрудника";
                    ViewBag.companies = new SelectList(db.Companies, "Id", "Name");
                    return View(emp);
                }
            }
            return NotFound();
        }

        [HttpPost]
        public async Task<IActionResult> Edit(Emp emp)
        {
            Company company = db.Companies.FirstOrDefault(c => c.Id == emp.CompanyID);
            db.Emps.Update(emp);
            await db.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        [HttpGet]
        [ActionName("DeleteEmp")]
        public async Task<IActionResult> ConfirmDelete(int? id)
        {
            if (id != null)
            {
                Emp emp = await db.Emps.FirstOrDefaultAsync(c => c.Id == id);
                emp.Company = await db.Companies.FirstOrDefaultAsync(c => c.Id == emp.CompanyID);
                if (emp != null)
                {
                    return View(emp);
                }
            }
            return NotFound();
        }

        [HttpPost]
        public async Task<IActionResult> DeleteEmp(int? id)
        {
            if (id != null)
            {
                Emp emp = await db.Emps.FirstOrDefaultAsync(c => c.Id == id);
                if (emp != null)
                {
                    db.Emps.Remove(emp);
                    await db.SaveChangesAsync();
                    return RedirectToAction("Index");
                }
            }
            return NotFound();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
