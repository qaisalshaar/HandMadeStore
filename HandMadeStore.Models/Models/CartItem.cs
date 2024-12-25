using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HandMadeStore.Models.Models
{
    public class CartItem
    {

        public int Id { get; set; }
        public int productId { get; set; }
        [ValidateNever]
        // navigation Propertie
        public Product Product { get; set; }
        [Range(1,int.MaxValue)]
        public int ProductCount { get; set; }

        // الان اربطه مع اليوزر
        public string ApplicationUserId { get; set; }
        // navigation Propertie
        [ValidateNever]
        public ApplicationUser ApplicationUser { get; set; }
        // لتحديد سعر كل منتج بحسب العدد
      // لاستبعادها من الميكريشن
        [NotMapped]
        public double PriceFromCount { get; set; }
    }
}
