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
using WasteCollection.Models.ViewModels;

namespace WasteCollection.Controllers
{
    public class EmployeeController : Controller
    {
        private readonly ApplicationDbContext _context;

        public EmployeeController(ApplicationDbContext context)
        {
            _context = context;
        }

        public ActionResult Index()
        {
            var userId = this.User.FindFirstValue(ClaimTypes.NameIdentifier);
            var employee = _context.Employees.Where(c => c.IdentityUserId == userId).SingleOrDefault();
            ResetPickUpConfirmation();

            if (employee == null)
            {
                return RedirectToAction("Create");

            }
            else
            {
                var customers = _context.Customers.Include(c => c.PickUpDay).ToList();
                var customersInDesignatedZipCode = customers.Where(c => c.ZipCode == employee.DesignatedZipCode && c.ConfirmTrashPickUp == false).ToList();
                var dayOfWeekString = DateTime.Now.DayOfWeek.ToString();
                var todayString = DateTime.Today.ToString();
                var today = DateTime.Today;
                SetAdditionalPickUpDay(customersInDesignatedZipCode);
                var customersInDesignatedZipCodeAndToday = customersInDesignatedZipCode.Where(c => c.PickUpDay.Date == dayOfWeekString || c.AdditionalPickUDayString == todayString).ToList();
                SetAccountSuspensionDates(customersInDesignatedZipCodeAndToday);
                var customersWithoutAccountsSuspended = customersInDesignatedZipCodeAndToday.Where(c => (c.AccountSuspendStartDate == null && c.AccountSuspendEndDate == null) || c.AccountSuspendStartDate > today || c.AccountSuspendEndDate < today).ToList();
                return View(customersWithoutAccountsSuspended);
            }
        }
        private void SetAdditionalPickUpDay(List<Customer> customers)
        {
            foreach (Customer customer in customers)
            {
                if (customer.AdditionalPickUDay.HasValue == true)
                {
                    var customerDate = customer.AdditionalPickUDay.Value.Date.ToString();
                    customer.AdditionalPickUDayString = customerDate;
                }
            }
        }
        private void SetAccountSuspensionDates(List<Customer> customers)
        {
            foreach (Customer customer in customers)
            {
                if (customer.AccountSuspendStartDate.HasValue == true && customer.AccountSuspendEndDate.HasValue == true)
                {
                    TimeSpan ts = new TimeSpan(0, 0, 0);
                    var startTime = customer.AccountSuspendStartDate.Value.Date + ts;
                    customer.AccountSuspendStartDate = startTime;
                    var endTime = customer.AccountSuspendEndDate.Value.Date + ts;
                    customer.AccountSuspendEndDate = endTime;

                }
            }
        }
        public ActionResult Filter() // get
        {
            CustomersByPickUpDay customersList = new CustomersByPickUpDay();
            var userId = this.User.FindFirstValue(ClaimTypes.NameIdentifier);
            var employee = _context.Employees.Where(c => c.IdentityUserId == userId).SingleOrDefault();
            var customers = _context.Customers.Include(c => c.PickUpDay).ToList();
            customersList.Customers = customers.Where(c => c.ZipCode == employee.DesignatedZipCode).ToList();
            customersList.PickUpDaySelection = new SelectList(_context.PickUpDays, "Date", "Date");
            return View(customersList);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Filter(CustomersByPickUpDay customer)
        {

            CustomersByPickUpDay customersList = new CustomersByPickUpDay();
            var userId = this.User.FindFirstValue(ClaimTypes.NameIdentifier);
            var employee = _context.Employees.Where(c => c.IdentityUserId == userId).SingleOrDefault();

            var selection = customer.PickUpDaySelection;
            var customers = _context.Customers.Include(c => c.PickUpDay).ToList();
            customersList.Customers = customers.Where(c => c.ZipCode == employee.DesignatedZipCode && c.PickUpDay.Date == selection).ToList();
            customersList.PickUpDaySelection = new SelectList(_context.PickUpDays, "Date", "Date");

            return View("Filter", customersList);

        }






        // GET: Employee/Create
        public IActionResult Create()
        {
            ViewData["IdentityUserId"] = new SelectList(_context.Users, "Id", "Id");
            return View();
        }

        // POST: Employee/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,FirstName,LastName,DesignatedZipCode,IdentityUserId")] Employee employee)
        {
            if (ModelState.IsValid)
            {
                _context.Add(employee);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["IdentityUserId"] = new SelectList(_context.Users, "Id", "Id", employee.IdentityUserId);
            return View(employee);
        }

        // GET: Employee/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var employee = await _context.Employees.FindAsync(id);
            if (employee == null)
            {
                return NotFound();
            }
            ViewData["IdentityUserId"] = new SelectList(_context.Users, "Id", "Id", employee.IdentityUserId);
            return View(employee);
        }

        // POST: Employee/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,FirstName,LastName,DesignatedZipCode,IdentityUserId")] Employee employee)
        {
            if (id != employee.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(employee);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!EmployeeExists(employee.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["IdentityUserId"] = new SelectList(_context.Users, "Id", "Id", employee.IdentityUserId);
            return View(employee);
        }

        // GET: Employee/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var employee = await _context.Employees
                .Include(e => e.IdentityUser)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (employee == null)
            {
                return NotFound();
            }

            return View(employee);
        }

        // POST: Employee/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var employee = await _context.Employees.FindAsync(id);
            _context.Employees.Remove(employee);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool EmployeeExists(int id)
        {
            return _context.Employees.Any(e => e.Id == id);
        }
        private void ResetPickUpConfirmation()
        {
            var customers = _context.Customers.Include(m => m.PickUpDay).ToList();
            DateTime today = DateTime.Today;
            DateTime yesterday = today.AddDays(-1);

            foreach (Customer customer in customers)
            {
                if (customer.MostRecentChargedDay == yesterday)
                {
                    customer.ConfirmTrashPickUp = false;
                }
            }
        }
    }
}
