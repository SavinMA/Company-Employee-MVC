using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
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

        public async Task<IActionResult> Index(SortState sortOrder = SortState.NameAsc)
        {
            IQueryable<Emp> emps = db.Emps.Include(x => x.Company);

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

            return View(await emps.AsNoTracking().ToListAsync());
        }

        public IActionResult Company()
        {
            ViewData["Message"] = "Компании";
            return View(db.Companies);
        }

        //ToDo add controller EDIT & DELETE
        public async Task<IActionResult> Edit(int? id)
        {
            if (id != null)
            {
                Emp emp = await db.Emps.FirstOrDefaultAsync(p => p.Id == id);
                if (emp != null)
                {
                    ViewData["Title"] = "Редактирование сотрудника";
                    ViewBag.company = db.Companies.ToList();
                    return View(emp);
                }
            }
            return NotFound();
        }

        [HttpPost]
        public async Task<IActionResult> Edit(Emp emp, string compName)
        {
            Company company = await db.Companies.FirstOrDefaultAsync(x => x.Name == compName);
            emp.CompanyID = company.Id;
            db.Emps.Update(emp);
            await db.SaveChangesAsync();
            return RedirectToAction("Index");
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
