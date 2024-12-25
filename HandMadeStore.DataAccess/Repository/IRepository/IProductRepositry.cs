using HandMadeStore.Models.Models;
using HandMadeStore.Models.Models.ViewModels;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HandMadeStore.DataAccess.Repository.IRepository
{
    public interface IProductRepositry:IRepository<Product>

    {
        void Update(Product product);

       void ResizeImage(ProductVM productVM, IFormFile imagurl);



    }
}
