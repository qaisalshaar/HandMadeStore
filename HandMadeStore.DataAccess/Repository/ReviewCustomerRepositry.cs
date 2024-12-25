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
    public class ReviewCustomerRepositry : Repository<ReviewCustomer>, IReviewCustomerRepositry
    {
        private readonly ApplicationDbContext _db;

        public ReviewCustomerRepositry(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

       

        public void Update(ReviewCustomer reviewcustomer)
        {
            // not this code   _db.Update(barnd);
            // this code with dbset review
            _db.ReviewCustomers.Update(reviewcustomer);
        }
    }
}
