using HandMadeStore.DataAccess.Data;
using HandMadeStore.DataAccess.Repository.IRepository;
using HandMadeStore.Models.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace HandMadeStore.DataAccess.Repository
{
    public class OrderHeaderRepositry : Repository<OrderHeader>, IOrderHeaderRepositry
    {
        private readonly ApplicationDbContext _db;

        public OrderHeaderRepositry(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

       

        public void Update(OrderHeader orderheader)
        {
            // not this code   _db.Update(orderheader);
            // this code with dbset Or
            _db.OrderHeaders.Update(orderheader);
        }

        public void UpdateOrderPayment(int id, string sessionId, string paymentIntentId)
        {
        var orderheader=  _db.OrderHeaders.FirstOrDefault(o => o.Id == id);

            orderheader.PaymentDate = DateTime.Now;
            orderheader.PaymentIntentId = paymentIntentId;
            orderheader.SessionId = sessionId;
        }

        public void UpdateStatus(int id, string orderstatus, string paymentstatus = null)
        {
           var orderheader=_db.OrderHeaders.FirstOrDefault(o=>o.Id == id);

            if (orderheader != null)
            {
                orderheader.OrderStatus = orderstatus;
                if(paymentstatus != null)  orderheader.PaymentStatus = paymentstatus;



            }
        }
    }
}
