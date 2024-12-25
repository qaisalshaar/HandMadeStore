using HandMadeStore.DataAccess.Repository;
using HandMadeStore.DataAccess.Repository.IRepository;
using HandMadeStore.Models.Models;
using HandMadeStore.Models.Models.ViewModels;
using HandMadeStore.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.Extensions.Hosting;
using Rotativa.AspNetCore;
using Stripe.Climate;
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Reflection;

namespace HandMadeStore.UI.Areas.Admin.Controllers
{

    [Area("Admin")]
    [Authorize(Roles = SD.Role_Admin + "," + SD.Role_Moderator)]
    //[Route("api/Product")]
    //[ApiController]
    public class ProductController : Controller
    {
        private readonly IUnitOfWork _unitOf;
        private readonly IWebHostEnvironment _host;

        // IWebHostEnvironment  to get www root path

        public ProductController(IUnitOfWork unitOf, IWebHostEnvironment host)
        {
            _unitOf = unitOf;
            _host = host;
        }

        public IActionResult Index()
        {
            //IEnumerable<Product> product = _unitOf.Product.GetAll() ;


            //return View(product);
            //return new JsonResult(product);


            // working with datatable
            


            return View();
        }


        // <i class="bi bi-plus-square"></i> &nbsp;  Create Product
        //[HttpGet]
        //public IActionResult Create()
        //{


        //    return View();
        //}
        //[HttpPost]
        //public IActionResult Create(Product product)
        //{
        //    if (!string.IsNullOrEmpty(product.Name))
        //    {
        //        var dublicateproduct = _unitOf.Product.GetFirstOrDefault(a => a.Name.ToLower() == product.Name.ToLower());

        //        if (dublicateproduct != null)
        //        {
        //            // show message for duplicate product name

        //            //ModelState.AddModelError("Name", "the product name is duplicated ");
        //            ModelState.AddModelError(string.Empty, "the product name is duplicated ");
        //        }


        //    }

        //    if (ModelState.IsValid)
        //    {
        //        _unitOf.Product.Add(product);
        //        _unitOf.Save();
        //        TempData.Add("success", "Product Saved Sucsusfully");
        //        return RedirectToAction("Index");

        //    }
        //    return View(product);
        //}



        //   Upsert Product
        [HttpGet]
        public IActionResult Upsert(int? id)
        {

            ProductVM productVM = new()
            {
                Product = new(),
                Categorylist = _unitOf.Category.GetAll()
                .Select(c => new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem
                {
                    Text = c.Name,
                    Value = c.Id.ToString(),

                }),

                Brandlist = _unitOf.Brand.GetAll()
                .Select(c => new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem
                {
                    Text = c.Name,
                    Value = c.Id.ToString(),

                }),

            };



            //Product product= new ();

            // Get all Category 
            //IEnumerable<SelectListItem> Categorylist = _unitOf.Category.GetAll()
            //    .Select(c => new SelectListItem
            //    {
            //        Text= c.Name,
            //        Value=c.Id.ToString(),

            //    });

            // Get all Brands 
            //IEnumerable<SelectListItem> Brandlist = _unitOf.Brand.GetAll()
            //    .Select(b => new SelectListItem
            //    {
            //        Text = b.Name,
            //        Value = b.Id.ToString(),

            //    });


            if (id == null || id == 0)
            {
                // Create Product
                //return View(product);
                //ViewBag.Categorylist= Categorylist;
                //ViewBag.Brandlist = Brandlist;
                //ViewData["Brandlist"] = Brandlist;
                //return View();
                return View(productVM);
            }
            else
            {
                // Update Product
                //var product = _unitOf.Product.GetFirstOrDefault(x => x.Id == id);
                //return View(product);

                productVM.Product = _unitOf.Product.GetFirstOrDefault(x => x.Id == id);
                return View(productVM);
            }


        }

        //POST
        [HttpPost]
        public IActionResult Upsert(ProductVM productVM, IFormFile imagurl)
        {
           

            if (ModelState.IsValid)
            {
             
                // Model state is Valid

                if (productVM.Product.Id == 0)
                {
                    string UPD = "".ToString();
                    UPD = "Create";
                    //Create new product
                    /////////////////////////////
                    ///
                    // Rezise Image
                    _unitOf.Product.ResizeImage( productVM, imagurl);


                    //////////////////////////
                    _unitOf.Product.Add(productVM.Product);
                    _unitOf.Save();
                    TempData["success"] = "Product created successfully";
                    return RedirectToAction("Index");
                }
                else
                {
                    string UPD = "".ToString();
                    UPD = "update";
                    // update product

                    // check for image is not changed
                    if (imagurl != null)
                    {
                        string RootPath = _host.WebRootPath;
                        //Delete old image imagurl if exists
                        if (productVM.Product.ImageUrl != null)
                        {
                            var oldImagePath = Path.Combine(RootPath, productVM.Product.ImageUrl
                                .TrimStart('\\'));
                            if (System.IO.File.Exists(oldImagePath))
                            {
                                System.IO.File.Delete(oldImagePath);
                            }
                        }

                        // Rezise Image
                        _unitOf.Product.ResizeImage(productVM, imagurl);






                        //Update product
                        _unitOf.Product.Update(productVM.Product);
                        _unitOf.Save();
                        TempData["success"] = "Product updated successfully";

                    }
                    else
                    {
                        // image is null
                        _unitOf.Product.Update(productVM.Product);
                        _unitOf.Save();
                        TempData["success"] = "Product updated successfully";

                    }
                            return RedirectToAction("Index");
                }



            }
            else
            { // Model state Not Valid




                productVM.Categorylist = _unitOf.Category.GetAll()
            .Select(c => new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem
            {
                Text = c.Name,
                Value = c.Id.ToString(),

            });

                productVM.Brandlist = _unitOf.Brand.GetAll()
             .Select(c => new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem
             {
                 Text = c.Name,
                 Value = c.Id.ToString(),

             });

                return View(productVM);







            }
        }

    





        public JsonResult ProductNameCkeck(ProductVM productVM, string proname,string UPD)

        {


            System.Threading.Thread.Sleep(200);
            var checkproductname = _unitOf.Product.GetFirstOrDefault(x => x.Name == proname);

            //var checkproductname = _unitOf.Product.GetFirstOrDefault(p => p.Name == proname).Name;
            if (checkproductname != null)
            {
                return Json(1);


            }
            else
            {
                return Json(0);

            }

        }



        public JsonResult ProductImageCkeck(ProductVM productvm, InputType imagurl)

        {


            System.Threading.Thread.Sleep(200);

            var checkselectimage = _unitOf.Product.GetFirstOrDefault(p => p.ImageUrl == imagurl.ToString());
            if (imagurl.ToString() != null)
            {
                return Json(1);


            }
            else
            {
                return Json(0);

            }

        }
    

        public IActionResult Print(int id)
        {
            // الحصول على معلومات المنتج
            var product=_unitOf.Product.GetFirstOrDefault(p=>p.Id == id,inclodePropertiies:"Brand,Category");
            // الحصول على عدد الرفيوز من المستخدمين للمنتج
            var reviewscount=_unitOf.ReviewCustomer.GetAll(r=>r.ProductId==id).Count();
           // لكي استقبلها داخل الفيو
            TempData["Revcount"] =reviewscount.ToString();
            // نريد الفيو يتحول الى pdf
            return new ViewAsPdf(product){
                PageOrientation=Rotativa.AspNetCore.Options.Orientation.Landscape,
                // واذا اريد تحميلها على الجهاز
                //FileName=$"{product.Name}.pdf",
             //   FileName=$"{product.Name}.pdf",
            };

        }

        #region  API ENDPOINT
        // Get All product EndPoint
        public JsonResult GetAll()
        {
            // to get category name or Brand name
            var product = _unitOf.Product.GetAll(inclodePropertiies: "Category,Brand");

            return Json(new { data = product });


        }


        // Delete product EndPoint
   //    [HttpDelete]
        public IActionResult Delete(int? id)
        {
            var product = _unitOf.Product.GetFirstOrDefault(p => p.Id == id);
            if (product == null)
            {
                return Json(new { success = false, message = "Error On Deleting Product" });


            }

            // delete image from the folder
            var oldimagepath = Path.Combine(_host.WebRootPath, product.ImageUrl.TrimStart('\\'));

            if (System.IO.File.Exists(oldimagepath))
            {
                System.IO.File.Delete(oldimagepath);

            }
            _unitOf.Product.Remove(product);
            _unitOf.Save();
       //   TempData.Add("success", "Product Deleted Sucsusfully");

            // return RedirectToAction("Index");
           // System.Threading.Thread.Sleep(2000);
             return Json(new { success = true, message = "Product Deleted Sucsusfully" });
          

        }



       

        #endregion


    }
}
