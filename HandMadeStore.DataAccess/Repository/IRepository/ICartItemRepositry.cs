using HandMadeStore.Models.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HandMadeStore.DataAccess.Repository.IRepository
{
    public interface ICartItemRepositry : IRepository<CartItem>

    {
        //   حساب زيادة ونقصان في CartItem 
        void IncrementCount(CartItem cartItem,int amount);
       void DcrementCount(CartItem cartItem,int amount);
        // جلب عدد القطع الموجودة في سلة المشتريات
       int GetPiecesCount();




    }
}
