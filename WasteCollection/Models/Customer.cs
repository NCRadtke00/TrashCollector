using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace WasteCollection.Models
{
    public class Customer
    {
        [Key]
        public int Id { get; set; }
        [Display(Name = "First Name")]
        public string FirstName { get; set; }
        [Display(Name = "Last Name")]
        public string LastName { get; set; }
        [Display(Name = "Street Address")]
        public string StreetAddress { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        [Display(Name = "Zip Code")]
        public int ZipCode { get; set; }

        [ForeignKey("PickUpDay")]
        [Display(Name = "Pickup Day")]
        public int PickUpDayId { get; set; }
        public PickUpDay PickUpDay { get; set; }
        [Display(Name = "Additional PickUp date")]
        [DisplayFormat(DataFormatString = "{0:MM/dd/yyyy}")]
        public DateTime? AdditionalPickUDay { get; set; }
        [NotMapped]
        public string AdditionalPickUDayString { get; set; }
        [NotMapped]
        public SelectList Days { get; set; }
        public bool IsAccountSuspended { get; set; }

        [Display(Name = "Account Suspension Start Date")]
        [DisplayFormat(DataFormatString = "{0:MM/dd/yyyy}")]
        public DateTime? AccountSuspendStartDate { get; set; }

        [Display(Name = "Account Suspension End Date")]
        [DisplayFormat(DataFormatString = "{0:MM/dd/yyyy}")]
        public DateTime? AccountSuspendEndDate { get; set; }

        [NotMapped]
        public DateTime? MostRecentChargedDay { get; set; }
        [Display(Name = "Current Balance")]
        [DisplayFormat(DataFormatString = "{0:C}")]
        public double CurrentAccountBalance { get; set; }
        [Display(Name = "Check the box to confirm trash has been pick up!")]
        public bool ConfirmTrashPickUp { get; set; }


        [ForeignKey("IdentityUser")]
        public string IdentityUserId { get; set; }
        public IdentityUser IdentityUser { get; set; }
    }
}
