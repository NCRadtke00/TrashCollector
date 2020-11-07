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
        private readonly ApplicationDbContext _db;

        public EmployeeController(ApplicationDbContext context)
        {
            _db = context;
        }

        public ActionResult Index()
        {
            var userId = this.User.FindFirstValue(ClaimTypes.NameIdentifier);
            var employee = _db.Employees.Where(c => c.IdentityUserId == userId).SingleOrDefault();
            ResetPickUp();

            if (employee == null)
            {
                return RedirectToAction("Create");

            }
            else
            {
                var customers = _db.Customers.Include(c => c.PickUpDay).ToList();
                var customersInEmployeeZipCode = customers.Where(c => c.ZipCode == employee.DesignatedZipCode && c.ConfirmTrashPickUp == false).ToList();
                var dayOfWeekString = DateTime.Now.DayOfWeek.ToString();
                var todayString = DateTime.Today.ToString();
                var today = DateTime.Today;
                SetAdditionalPickUpDay(customersInEmployeeZipCode);
                var customersInEmployeeZipCodeAndToday = customersInEmployeeZipCode.Where(c => c.PickUpDay.Date == dayOfWeekString || c.AdditionalPickUp == todayString).ToList();
                SetAccountSuspensionDates(customersInEmployeeZipCodeAndToday);
                var customersWithoutAccountsSuspended = customersInEmployeeZipCodeAndToday.Where(c => (c.AccountSuspendStartDate == null && c.AccountSuspendEndDate == null) || c.AccountSuspendStartDate > today || c.AccountSuspendEndDate < today).ToList();
                return View(customersWithoutAccountsSuspended);
            }
        }
        private void SetAdditionalPickUpDay(List<Customer> customers)
        {
            foreach (Customer customer in customers)
            {
                if (customer.AdditionalPickUpDay.HasValue == true)
                {
                    var customerDate = customer.AdditionalPickUpDay.Value.Date.ToString();
                    customer.AdditionalPickUp = customerDate;
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
        public ActionResult FilterResults() // get
        {
            CustomersByPickUpDay customersList = new CustomersByPickUpDay();
            var userId = this.User.FindFirstValue(ClaimTypes.NameIdentifier);
            var employee = _db.Employees.Where(c => c.IdentityUserId == userId).SingleOrDefault();
            var customers = _db.Customers.Include(c => c.PickUpDay).ToList();
            customersList.Customers = customers.Where(c => c.ZipCode == employee.DesignatedZipCode).ToList();
            customersList.PickUpDaySelection = new SelectList(_db.PickUpDays, "Date", "Date");
            return View(customersList);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult FilterResults(CustomersByPickUpDay customer)
        {
            CustomersByPickUpDay customersPickUpList = new CustomersByPickUpDay();
            var userId = this.User.FindFirstValue(ClaimTypes.NameIdentifier);
            var employee = _db.Employees.Where(c => c.IdentityUserId == userId).SingleOrDefault();
            var selected = customer.PickUpDaySelected;
            var customers = _db.Customers.Include(c => c.PickUpDay).ToList();
            customersPickUpList.Customers = customers.Where(c => c.ZipCode == employee.DesignatedZipCode && c.PickUpDay.Date == selected).ToList();
            customersPickUpList.PickUpDaySelection = new SelectList(_db.PickUpDays, "Date", "Date");
            return View("Filter", customersPickUpList);
        }

        public ActionResult Create()
        {
            Employee employee = new Employee();
            return View(employee);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Employee employee)
        {
            try
            {
                var userId = this.User.FindFirstValue(ClaimTypes.NameIdentifier);
                employee.IdentityUserId = userId;
                _db.Add(employee);
                _db.SaveChanges();
                return RedirectToAction("Index");
            }
            catch
            {
                return View(employee);
            }
        }

        private void ResetPickUp()
        {
            var customers = _db.Customers.Include(m => m.PickUpDay).ToList();
            DateTime today = DateTime.Today;
            DateTime yesterday = today.AddDays(-1);
            foreach (Customer customer in customers)
            {
                if (customer.DateCharged == yesterday)
                {
                    customer.ConfirmTrashPickUp = false;
                }
            }
        }

        public ActionResult Confirm(int id)
        {
            var customer = _db.Customers.Where(c => c.Id == id).Single();
            return View(customer);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Confirm(Customer customer)
        {
            try
            {
                if (customer.ConfirmTrashPickUp == true)
                {
                    customer.DateCharged = DateTime.Today;
                    customer.CurrentAccountBalance += 50.00;
                }
                _db.Update(customer);
                _db.SaveChanges();
                return RedirectToAction("Index");
            }
            catch
            {
                return View("Index");
            }
        }
        public ActionResult Map(int id) // for adding google maps
        {
            return View();
        }
    }
}
