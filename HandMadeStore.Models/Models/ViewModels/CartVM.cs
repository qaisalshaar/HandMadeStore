using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HandMadeStore.Models.Models.ViewModels
{
    public class CartVM
    {

        public IEnumerable<CartItem> CartItems { get; set; }



       
        // اضافة OrderHeader
        public OrderHeader orderheader { get; set; }

        // اضافة سعر اجمالي قيمة سلة المشتريات
        public double CartTotal { get; set; }
        // اجمالي عدد القطع في سلة المشتريات
        public int ItemPeicesCount { get; set; }
    }
}
