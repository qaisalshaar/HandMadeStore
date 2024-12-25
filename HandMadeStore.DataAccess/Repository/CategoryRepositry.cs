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
    public class CategoryRepositry : Repository<Category>, ICategoryRepositry
    {
        private readonly ApplicationDbContext _db;

        public CategoryRepositry(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

       

        public void Update(Category category)
        {
            // not this code   _db.Update(category);
            // this code with dbset Categories
            _db.Categories.Update(category);
        }
    }
}
