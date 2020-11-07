using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;


namespace WasteCollection.Models
{
    public class Employee
    {
        [Key]
        public int Id { get; set; }
    }
}
