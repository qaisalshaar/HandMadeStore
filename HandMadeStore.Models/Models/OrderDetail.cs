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
    public class OrderDetail
    {

        public int Id { get; set; }
        [Required]
        public int OrderId { get; set; }
        [ValidateNever]
        [ForeignKey("OrderId")]
        public OrderHeader OrderHeader { get; set; }

        [Required]
        public int ProductId { get; set; }
        [ValidateNever]
        public Product Product { get; set; }

        public int Count { get; set; }
        // في حال تم عمل طلبية ولكن لم يتم دفع قيمة الطلبية ومكن السعر يتغير بحسب وقت الدفع
        public double Price { get; set; }
    }
}
