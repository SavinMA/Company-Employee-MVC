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
        public async Task<ActionResult> Index(int? company, string name, SortState sortOrder = SortState.NameAsc, int page = 1)
        {
            int pageSize = 5;

            //фильтрация
            IQueryable<Emp> emps = db.Emps.Include(p => p.Company);
            if (company != null && company != 0)
                emps = emps.Where(p => p.CompanyID == company);
            if (!string.IsNullOrEmpty(name))
                emps = emps.Where(p => p.Name.Contains(name));


            /*ViewData["NameSort"] = sortOrder == SortState.NameAsc ? SortState.NameDesc : SortState.NameAsc;
            ViewData["AgeSort"] = sortOrder == SortState.AgeAsc ? SortState.AgeDesc : SortState.AgeAsc;
            ViewData["CompSort"] = sortOrder == SortState.CompanyAsc ? SortState.CompanyDesc : SortState.CompanyAsc;
            */

            //сортировка
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

            //пагинация
            var count = await emps.CountAsync();
            var items = emps.Skip((page - 1) * pageSize).Take(pageSize).AsQueryable();


            //формируем модель представления
            IndexViewModel viewModel = new IndexViewModel
            {
                PageViewModel = new PageViewModel(count, page, pageSize),
                SortViewModel = new SortViewModel(sortOrder),
                FilterViewModel = new FilterViewModel(db.Companies.ToList(), company, name),
                Emps = items
            };

            return View(viewModel);
        }

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
