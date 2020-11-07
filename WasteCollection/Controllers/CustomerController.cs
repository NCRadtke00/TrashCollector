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
        private readonly ApplicationDbContext _db;

        public CustomerController(ApplicationDbContext db)
        {
            _db = db;
        }

        public IActionResult Index()
        {
            var userId = this.User.FindFirstValue(ClaimTypes.NameIdentifier);
            var customer = _db.Customers.Where(c => c.IdentityUserId == userId).SingleOrDefault();
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
            customer.PickUpDay = _db.PickUpDays.Find(customer.PickUpDayId);
            if (customer == null)
            {
                return NotFound();
            }
            return View(customer);
        }
       
        public IActionResult Create()
        {
            var days = _db.PickUpDays.ToList();
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
                customer.PickUpDay = _db.PickUpDays.Find(customer.PickUpDayId);
                _db.Add(customer);
                _db.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(customer);
        }

        public IActionResult Edit(int? id)
        {

            var customer = _db.Customers.SingleOrDefault(mbox => mbox.Id == id);
            var days = _db.PickUpDays.ToList();
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
                _db.Update(customer);
                _db.SaveChanges();
                return RedirectToAction("Details", customer);
            }
            catch
            {
                return RedirectToAction("Index");
            }
        }
    }
}
