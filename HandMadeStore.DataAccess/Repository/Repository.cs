using HandMadeStore.DataAccess.Data;
using HandMadeStore.DataAccess.Repository.IRepository;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;






namespace HandMadeStore.DataAccess.Repository
{
    public class Repository<T> : IRepository<T> where T : class
    {
        private readonly ApplicationDbContext _db;

        internal DbSet<T> _dbset;
      
    public Repository(ApplicationDbContext db)
        {
            _db = db;
            _dbset = _db.Set<T>();

        }
     

        public void Add(T entity)
        {
         _db.Add(entity);
        }

        public void ClearChangingTracking()
        {
            _db.ChangeTracker.Clear();
        }
        //inclodePropertiies="Category,Brand) + FILTER
        public IEnumerable<T> GetAll(Expression<Func<T, bool>> flitter=null, string inclodePropertiies = null)
        {
            IQueryable<T> query = _dbset.AsQueryable();
            if(flitter != null) query = query.Where(flitter);
            if(inclodePropertiies != null)
            {
                //inclodePropertiies="Category,Brand)
                foreach (var inclod in inclodePropertiies.Split(new char[] {','},
                   StringSplitOptions.RemoveEmptyEntries) ) {

                    query = query.Include(inclod);


                }
            }

            return query.ToList();
        }
        // return just one entity
        public T GetFirstOrDefault(Expression<Func<T, bool>> flitter, string inclodePropertiies = null)
        {
           
            try {

                IQueryable<T> query = _dbset.AsQueryable();



                query = query.Where(flitter);
                if (inclodePropertiies != null)
                {
                    //inclodePropertiies="Category,Brand)
                    foreach (var inclod in inclodePropertiies.Split(new char[] { ',' },
                       StringSplitOptions.RemoveEmptyEntries))
                    {

                        query = query.Include(inclod);


                    }
                }
                return query.FirstOrDefault();


            }


            catch (Exception ex)
            {


              return null;

              

            }




           
        }

        public void Remove(T entity)
        {
            
            _db.Remove(entity);
        }

        public void RemoveRange(IEnumerable<T> entity)
        {
            _db.RemoveRange(entity);
        }
    }
}
