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
    public class BrandRepositry : Repository<Brand>, IBrandRepositry
    {
        private readonly ApplicationDbContext _db;

        public BrandRepositry(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

       

        public void Update(Brand brand)
        {
            // not this code   _db.Update(barnd);
            // this code with dbset brand
            _db.Brands.Update(brand);
        }
    }
}
