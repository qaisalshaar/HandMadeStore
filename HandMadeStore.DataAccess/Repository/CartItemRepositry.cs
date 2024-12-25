using HandMadeStore.DataAccess.Data;
using HandMadeStore.DataAccess.Repository.IRepository;
using HandMadeStore.Models.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace HandMadeStore.DataAccess.Repository
{
    public class CartItemRepositry : Repository<CartItem>, ICartItemRepositry
    {
        private readonly ApplicationDbContext _db;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CartItemRepositry(ApplicationDbContext db,IHttpContextAccessor httpContextAccessor) : base(db)
        {
            _db = db;
            _httpContextAccessor = httpContextAccessor;
        }

        public void DcrementCount(CartItem cartItem, int amount)
        {// تنقيص
           cartItem.ProductCount -= amount;
        }

        public int GetPiecesCount()
        {
            int PiecesCount = 0;
            //_httpContextAccessor للحصول على المستخدم الحالي داخل الربيوزتري نحتاج
            var userId = _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
            var cartItems = _db.CartItems.Where
                (c => c.ApplicationUserId == userId).ToList();
            if (cartItems != null)
            {
                foreach (var item in cartItems)
                {
                    PiecesCount += item.ProductCount;
                }
            }
            return PiecesCount;
        }

        public void IncrementCount(CartItem cartItem, int amount)
        {// زيادة
            cartItem.ProductCount += amount;
        }
    }
}
