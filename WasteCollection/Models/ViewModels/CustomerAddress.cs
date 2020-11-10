using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;


namespace WasteCollection.Models.ViewModels
{
      
    public class CustomerAddress
    {
        [Key]
        public int Id { get; set; }

        [Display(Name = "Street Address")]
        public string StreetAddress { get; set; }
        public string State { get; set; }
        public string City { get; set; }

        [Display(Name = "Zip Code")]
        public int ZipCode { get; set; }

        public double Longitude { get; set; }
        public double Latitude { get; set; }
    }
}
