using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace HandMadeStore.Models.Models.ViewModels
{
    public class ProductVM
    {
       
        public Product Product { get; set; }

        [ValidateNever]
 
        public IEnumerable<SelectListItem> Categorylist  { get; set; }
        [ValidateNever]
        public IEnumerable<SelectListItem> Brandlist { get; set; }


    }
}
