using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WasteCollection.Data;
using WasteCollection.Models;

namespace WasteCollection.Controllers
{
    public class CustomerController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CustomerController(ApplicationDbContext db)
        {
            _context = db;
        }

        public IActionResult Index()
        {
            var userId = this.User.FindFirstValue(ClaimTypes.NameIdentifier);
            var customer = _context.Customers.Where(c => c.IdentityUserId == userId).SingleOrDefault();
            if (customer == null)
            {
                return RedirectToAction("Create");
            }
            else
            {
                return View("Details", customer);
            }
        }
        public IActionResult Details(Customer customer)
        {
            customer.PickUpDay = _context.PickUpDays.Find(customer.PickUpDayId);
            if (customer == null)
            {
                return NotFound();
            }
            return View(customer);
        }
       
        public IActionResult Create()
        {
            var days = _context.PickUpDays.ToList();
            Customer customer = new Customer()
            {
                Days = new SelectList(days, "Id", "Date")
            };
            return View(customer);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Customer customer)
        {
            if (ModelState.IsValid)
            {
                var userId = this.User.FindFirstValue(ClaimTypes.NameIdentifier);
                customer.IdentityUserId = userId;
                customer.PickUpDay = _context.PickUpDays.Find(customer.PickUpDayId);
                _context.Add(customer);
                _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(customer);
        }

        public IActionResult Edit(int? id)
        {

            var customer = _context.Customers.SingleOrDefault(c => c.Id == id);
            var days = _context.PickUpDays.ToList();
            customer.Days = new SelectList(days, "Id", "Date");
            if (customer == null)
            {
                return NotFound();
            }
            return View(customer);
        }
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Customer customer)
        {
            try
            {
                _context.Update(customer);
                _context.SaveChanges();
                return RedirectToAction("Details", customer);
            }
            catch
            {
                return RedirectToAction("Index");
            }
        }
    }
}
