using HandMadeStore.Models.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HandMadeStore.DataAccess.Repository.IRepository
{
    public interface IUnitOfWork
    {
        // Insert Specific Repositry
        //  Signature  for Properties
        ICategoryRepositry Category { get; }
        IBrandRepositry Brand { get; }
        IProductRepositry Product { get; }
        IShopRepositry Shop { get; }
        ICartItemRepositry CartItem { get; }
        IApplicationUserRepositry ApplicationUser { get; }
        IOrderHeaderRepositry OrderHeader { get; }
        IOrderDetailRepositry OrderDetail { get; }
        IReviewCustomerRepositry ReviewCustomer { get; }
     
    
      



        // Insert Global Method
        //  Signature  for Method
        void Save();

    }
}
