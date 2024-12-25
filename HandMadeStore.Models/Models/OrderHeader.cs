using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HandMadeStore.Models.Models
{
    public class OrderHeader
    {
        public int Id { get; set; }
        public string ApplicationUserId { get; set; }
        [ValidateNever]
        public ApplicationUser ApplicationUser { get; set; }

        [Required]
        [DisplayFormat(DataFormatString= "{dd/MM/yyyy}")]
        public DateTime? OrderDate { get; set; }
        [DisplayFormat(DataFormatString = "{dd/MM/yyyy}")]
        public DateTime? ShippingDate { get; set; }
        public double OrderTotal { get; set; }
        public string OrderStatus { get; set; }
        public string PaymentStatus { get; set; }
        public string TrackingNumber { get; set; }
        // اسم شركة الشحن
        public string Carrier { get; set; }
        public DateTime? PaymentDate { get; set; }
        // في حالة الدفع بالاجل او تاريخ الاستحقاق
        [DisplayFormat(DataFormatString = "{dd/MM/yyyy}")]
        public DateTime? PaymentDueDate { get; set; }

        [Required]
        public string Name { get; set; }
        [Required]
        public string PhoneNumber { get; set; }
        [Required]
        public string StreetAddress { get; set; }
        [Required]
        public string City { get; set; }
        [Required]
        public string PostalCode { get; set; }

        //Stripe Payment Gateway
        public string SessionId { get; set; } = null;
        // رقم نية السداد او الدفع
        public string PaymentIntentId { get; set; }=null;


    }
}
