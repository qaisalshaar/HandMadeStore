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
    public class OrderDetailRepositry : Repository<OrderDetail>, IOrderDetailRepositry
    {
        private readonly ApplicationDbContext _db;

        public OrderDetailRepositry(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

       

        public void Update(OrderDetail orderdetails)
        {
            // not this code   _db.Update(orderdetails);
            // this code with dbset Categories
            _db.OrderDetails.Update(orderdetails);
        }
    }
}
