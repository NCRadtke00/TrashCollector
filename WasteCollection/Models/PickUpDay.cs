using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace WasteCollection.Models
{
    public class PickUpDay
    {
        [Key]
        public int Id { get; set; }
        [Display(Name = "Pickup Day")]
        public string Date { get; set; }
    }
}
