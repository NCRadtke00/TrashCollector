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

        // GET: Customer
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

        // GET: Customer/Details/5
        public IActionResult Details(Customer customer)
        {
            customer.PickUpDay = _db.PickUpDays.Find(customer.PickUpDayId);
            if (customer == null)
            {
                return NotFound();
            }

            return View(customer);
        }

        // GET: Customer/Create
        public IActionResult Create()
        {
            var days = _db.PickUpDays.ToList();
            Customer customer = new Customer()
            {
                Days = new SelectList(days, "Id", "Date")
            };
            return View(customer);
        }

        // POST: Customer/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
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

        // GET: Customer/Edit/5
        public IActionResult Edit(int? id)
        {

            var customer = _db.Customers.SingleOrDefault(mbox => mbox.Id == id);
            var days = _db.PickUpDays.ToList();
            customer.Days = new SelectList(days, "Id", "Date");

            //ViewData["IdentityUserId"] = new SelectList(_context.Users, "Id", "Id", customer.IdentityUserId);
            //ViewData["PickUpDayId"] = new SelectList(_context.PickUpDays, "Id", "Id", customer.PickUpDayId);
            if (customer == null)
            {
                return NotFound();
            }
            return View(customer);
        }

        // POST: Customer/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
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
        //ViewData["IdentityUserId"] = new SelectList(_context.Users, "Id", "Id", customer.IdentityUserId);
        //ViewData["PickUpDayId"] = new SelectList(_context.PickUpDays, "Id", "Id", customer.PickUpDayId);
        //    return View(customer);
    }

}
