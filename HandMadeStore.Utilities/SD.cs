using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HandMadeStore.Utilities
{
    public class SD
    {

        public const string Role_Admin = "Admin";
        // مشرف
        public const string Role_Moderator = "Moderator";
        public const string Role_Shop = "Shop";
        public const string Role_Customer = "Customer";


        // for Order Header And Details
        public const string StatusPending = "Pending";
        public const string StatusApproved = "Approved";
        public const string StatusInProcess = "Processing";
        public const string StatusShipped = "Shipped";
        public const string StatusCancelled = "Cancelled";
        public const string StatusRefunded = "Refunded";

        public const string PaymentStatusPending = "Pending";
        public const string PaymentStatusApproved = "Approved";
        public const string PaymentStatusDelayedPayment = "ApprovedForDelayedPayment";
        public const string PaymentStatusRejected = "Rejected";

        // الحصول على الدومين الخاص بالموقع
        public const string AppUrlDomain = "https://localhost:44397/";

  


        // Session CartSession
        public const string CartSession = "CartCountSession";
    }
}
