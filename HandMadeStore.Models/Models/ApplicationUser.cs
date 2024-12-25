using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HandMadeStore.Models.Models
{
    public class ApplicationUser:IdentityUser
    {
        [Required]
        public string Name { get; set; }
        public string City { get; set; }
        public string StreetName { get; set; }
        public string PostalCode { get; set; }
        // add shop properties
        public int? ShopId { get; set; }
        [ValidateNever]
        public Shop shop { get; set; }

    }
}
