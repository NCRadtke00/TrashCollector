﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;



namespace WasteCollection.Models.ViewModels
{
    public class CustomersByPickUpDay
    {
        public IEnumerable<Customer> Customers { get; set; }
        public SelectList PickUpDaySelections { get; set; }
    }
}