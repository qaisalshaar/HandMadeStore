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
    public class ApplicationUserRepositry : Repository<ApplicationUser>, IApplicationUserRepositry
    {
        private readonly ApplicationDbContext _db;

        public ApplicationUserRepositry(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

       

        
    }
}
