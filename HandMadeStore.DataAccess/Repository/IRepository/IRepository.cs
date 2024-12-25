using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
namespace HandMadeStore.DataAccess.Repository.IRepository
{ 
     
    
    public interface IRepository<T> where T : class
    {

      
        // to get category name or Brand name or Product
        //IEnumerable<T> GetAll(string inclodePropertiies=null);
        //to get category name or Brand name or Product with FILTER


        IEnumerable<T> GetAll(Expression<Func<T, bool>> flitter=null, string inclodePropertiies = null);





        T GetFirstOrDefault(Expression<Func<T, bool>> flitter,  string inclodePropertiies = null);
        void Add(T entity);
        void Remove(T entity);
        void RemoveRange(IEnumerable<T> entity);
        void ClearChangingTracking();


    }
}
