using HandMadeStore.DataAccess.Data;
using HandMadeStore.DataAccess.Repository.IRepository;
using HandMadeStore.Models.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HandMadeStore.DataAccess.Repository
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _db;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public ICategoryRepositry Category { get;private set; }
        public IBrandRepositry Brand { get;private set; }
        public IProductRepositry Product { get;private set; }
        public IShopRepositry Shop { get;private set; }
        public IApplicationUserRepositry ApplicationUser { get;private set; }
        public ICartItemRepositry CartItem { get;private set; }

        public IOrderHeaderRepositry OrderHeader { get; private set; }

        public IOrderDetailRepositry OrderDetail { get; private set; }
        public IReviewCustomerRepositry ReviewCustomer { get; private set; }
  

      

        public UnitOfWork(ApplicationDbContext db, IWebHostEnvironment host, IHttpContextAccessor httpContextAccessor)
        {
            _db = db;
           
            Category = new CategoryRepositry(db);
            Brand = new BrandRepositry(db);
            Product = new ProductRepositry(db,host );
            Shop =new  ShopRepositry(db);
            ApplicationUser =new  ApplicationUserRepositry(db);
            CartItem =new  CartItemRepositry(db, httpContextAccessor);
            OrderHeader =new  OrderHeaderRepositry(db);
            OrderDetail =new  OrderDetailRepositry(db);
            ReviewCustomer = new ReviewCustomerRepositry(db);
         
           
            


        }


        public void Save()
        {
          _db.SaveChanges();

        }
    }
}
