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
    public class ShopRepositry : Repository<Shop>, IShopRepositry
    {
        private readonly ApplicationDbContext _db;

        public ShopRepositry(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

       

        public void Update(Shop shop)
        {
            // not this code   _db.Update(barnd);
            // this code with dbset shop
            _db.Shops.Update(shop);
        }
    }
}
