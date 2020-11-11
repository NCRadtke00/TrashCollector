using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;



namespace WasteCollection.Models.ViewModels
{
    public class CustomersByPickUpDay
    {
        //[Key]
        //public int Id { get; set; }
        public SelectList DaySelection { get; set; } 
        [Display(Name = "Which day would you like to see?")]
        public string DaySelected { get; set; }
        public IEnumerable<Customer> Customers { get; set; }
    }
}
