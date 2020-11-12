using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using WasteCollection.Models;
using WasteCollection.Models.ViewModels;

namespace WasteCollection.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<Employee> Employees { get; set; }
        public DbSet<PickUpDay> PickUpDays { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<IdentityRole>()
                .HasData(
                    new IdentityRole()
                    {
                        Name = "Employee",
                        NormalizedName = "EMPLOYEE"
                    },
                    new IdentityRole()
                    {
                        Name = "Customer",
                        NormalizedName = "CUSTOMER"
                    });
            builder.Entity<Customer>()
                .HasData(
                    new Customer()
                    {
                        Id = 1,
                        LastName = "White",
                        FirstName = "Reggie",
                        CurrentAccountBalance = 50,
                        StreetAddress = "2848 n cramer st",
                        City = "milwaukee",
                        State = "Wisconsin",
                        ZipCode = 53211,
                        PickUpDayId = 1
                    });
            builder.Entity<PickUpDay>()
                .HasData(
                    new PickUpDay
                    {
                        Id = 1,
                        Date = "Monday"
                    },
                    new PickUpDay
                    {
                        Id = 2,
                        Date = "Tuesday"
                    },
                    new PickUpDay
                    {
                        Id = 3,
                        Date = "Wednesday"
                    },
                    new PickUpDay
                    {
                        Id = 4,
                        Date = "Thursday"
                    },
                    new PickUpDay
                    {
                        Id = 5,
                        Date = "Friday"
                    },
                    new PickUpDay
                    {
                        Id = 6,
                        Date = "Saturday"
                    },
                    new PickUpDay
                    {
                        Id = 7,
                        Date = "Sunday"
                    });
        }
    }
}
