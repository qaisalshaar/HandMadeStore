using HandMadeStore.DataAccess.Repository.IRepository;
using HandMadeStore.Models.Models;
using HandMadeStore.Models.Models.ViewModels;
using HandMadeStore.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.Extensions.Hosting;
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Reflection;

namespace HandMadeStore.UI.Areas.Admin.Controllers
{

    [Area("Admin")]
    [Authorize(Roles = SD.Role_Admin + "," + SD.Role_Moderator)]
    //[Route("api/Shop")]
    //[ApiController]
    public class ShopController : Controller
    {
        private readonly IUnitOfWork _unitOf;
       

     

        public ShopController(IUnitOfWork unitOf)
        {
            _unitOf = unitOf;
           
        }

        public IActionResult Index()
        {
                        return View();
        }


       


        //   Upsert Shop
        [HttpGet]
        public IActionResult Upsert(int? id)
        {

            Shop shop = new();
           

          

            
            if (id == null || id == 0)
            {
                
                return View(shop );
            }
            else
            {
                

                shop = _unitOf.Shop.GetFirstOrDefault(s => s.Id == id);
                return View(shop);
            }


        }

        //POST
        [HttpPost]
        public IActionResult Upsert(Shop shop)
        {

           


            if (ModelState.IsValid )
            {



                if (shop.Id == 0)
                {
               

                    //Create new shop
                    _unitOf.Shop.Add(shop);
                    _unitOf.Save();
                    TempData["success"] = "Shop created successfully";
                    return RedirectToAction("Index");
                }
                else
                { // update
                   


                        //Update shop
                        _unitOf.Shop.Update(shop);
                        _unitOf.Save();
                        TempData["success"] = "Shop updated successfully";
                        return RedirectToAction("Index");
                    }

                }


            
            return View(shop);
        }



        



        public JsonResult ShopNameCkeck(Shop shop , string shopname)

        {


            System.Threading.Thread.Sleep(200);
            var checkshopname = _unitOf.Shop.GetFirstOrDefault(s => s.Name == shopname);

            //var checkshopname = _unitOf.Shop.GetFirstOrDefault(p => p.Name == shopname).Name;
            if (checkshopname != null)
            {
                return Json(1);


            }
            else
            {
                return Json(0);

            }

        }



     

        #region  API ENDPOINT
        // Get All shop EndPoint
        public JsonResult GetAll()
        {
            // to get category name or Brand name
            var shop = _unitOf.Shop.GetAll();

            return Json(new { data = shop });


        }

        // Delete shop EndPoint
        [HttpDelete]
        public IActionResult Delete(int? id)
        {
            var shop = _unitOf.Shop.GetFirstOrDefault(s => s.Id == id);
            if (shop == null)
            {
                return Json(new { success = false, message = "Error On Deleting Shop" });


            }

           
            _unitOf.Shop.Remove(shop);
            _unitOf.Save();
           
            return Json(new { success = true, message = "Shop Deleted Sucsusfully" });
        }
        #endregion


    }
}
