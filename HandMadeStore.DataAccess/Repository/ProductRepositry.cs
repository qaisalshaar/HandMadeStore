using HandMadeStore.DataAccess.Data;
using HandMadeStore.DataAccess.Repository.IRepository;
using HandMadeStore.Models.Models;
using HandMadeStore.Models.Models.ViewModels;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.Drawing;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Hosting;
using static System.Net.Mime.MediaTypeNames;

namespace HandMadeStore.DataAccess.Repository
{
    public class ProductRepositry : Repository<Product>, IProductRepositry
    {
        private readonly ApplicationDbContext _db;
        private readonly IWebHostEnvironment _host;

        public ProductRepositry(ApplicationDbContext db, IWebHostEnvironment host) : base(db)
        {
            _db = db;
            _host = host;
            // to get category name or Brand name
            //_db.Products.Include(p => p.Category).Include(p => p.Brand);

        }

        public void ResizeImage(ProductVM productVM, IFormFile imagurl)
        {
            string RootPath = _host.WebRootPath;
            string imagurlName = Guid.NewGuid().ToString();
            var productsFolderPath = Path.Combine(RootPath, @"images\productimages");
            var extension = Path.GetExtension(imagurl.FileName);
            string imageoriginalpath = Path.Combine(RootPath, @"images\originalimage\" + imagurl.FileName);


            //start resize image 
            using (var originalImage = Image.FromFile(imageoriginalpath))
            {
                using (var resizedImage = new Bitmap(500, 500))
                {
                    using (var graphics = Graphics.FromImage(resizedImage))
                    {
                        graphics.DrawImage(originalImage, 0, 0, 500, 500);
                    }

                    // Save the resized image or return it as a response
                    // For example, save to a file:
                    resizedImage.Save(Path.Combine(RootPath, @"images\productimages\" + imagurlName + extension), ImageFormat.Jpeg);
                    //  resizedImage.Save("D:\\ASP Kais\\Products Data", ImageFormat.Jpeg);
                    productVM.Product.ImageUrl = @"\images\productimages\" + imagurlName + extension;

                }
            }
            //End resize image 
        }

        public void Update(Product product)
        {
            // not this code   _db.Update(barnd);
            // this code with dbset brand
            //_db.Products.Update(product);

            var ProductFromDb = _db.Products.Find(product.Id);

            if (ProductFromDb != null)
            {
                ProductFromDb.Name = product.Name;
                ProductFromDb.Description = product.Description;
                ProductFromDb.CategoryId = product.CategoryId;
                ProductFromDb.BrandId = product.BrandId;
                ProductFromDb.Price = product.Price;
                ProductFromDb.Price10plus = product.Price10plus;
                ProductFromDb.Price30plus = product.Price30plus;

                ProductFromDb.CreatedDate = product.CreatedDate;

                if (ProductFromDb.ImageUrl != null)
                {

                    ProductFromDb.ImageUrl = product.ImageUrl;

                }


            }
        }


    }
}