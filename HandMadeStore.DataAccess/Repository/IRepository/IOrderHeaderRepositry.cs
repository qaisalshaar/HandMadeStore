using HandMadeStore.Models.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HandMadeStore.DataAccess.Repository.IRepository
{
    public interface IOrderHeaderRepositry : IRepository<OrderHeader>

    {
        void Update(OrderHeader orderheader);
        // حالة الطلبية
        void UpdateStatus(int id, string orderstatus,string paymentstatus=null);

        // تحديث بيانات ال stripe id(SessionId) and PaymentIntentId
        //  orderheaderId == id جلب ال 
        void UpdateOrderPayment(int id, string sessionId,string paymentIntentId);




    }
}
