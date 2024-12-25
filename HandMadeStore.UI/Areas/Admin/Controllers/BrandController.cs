using HandMadeStore.DataAccess.Data;
using HandMadeStore.DataAccess.Repository;
using HandMadeStore.DataAccess.Repository.IRepository;
using HandMadeStore.Models.Models;
using HandMadeStore.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HandMadeStore.UI.Areas.Admin.Controllers
{

    [Area("Admin")]
    [Authorize(Roles = SD.Role_Admin + "," + SD.Role_Moderator)]
    public class BrandController : Controller
    {
        private readonly IUnitOfWork _unitOf;

        public BrandController(IUnitOfWork unitOf)
        {
            _unitOf = unitOf;
        }

        public IActionResult Index()
        {
            IEnumerable<Brand> brand = _unitOf.Brand.GetAll();


            return View(brand);
        }


        // <i class="bi bi-plus-square"></i> &nbsp;  Create Brand
        [HttpGet]
        public IActionResult Create()
        {


            return View();
        }
        [HttpPost]
        public IActionResult Create(Brand brand)
        {
            if (!string.IsNullOrEmpty(brand.Name))
            {
                var dublicatebrand = _unitOf.Brand.GetFirstOrDefault(a => a.Name.ToLower() == brand.Name.ToLower());

                if (dublicatebrand != null)
                {
                    // show message for duplicate brand name

                    //ModelState.AddModelError("Name", "the brand name is duplicated ");
                    ModelState.AddModelError(string.Empty, "the brand name is duplicated ");
                }


            }

            if (ModelState.IsValid)
            {
                _unitOf.Brand.Add(brand);
                _unitOf.Save();
                TempData.Add("success", "Brand Saved Sucsusfully");
                return RedirectToAction("Index");

            }
            return View(brand);
        }



        //   Update Brand
        [HttpGet]
        public IActionResult Update(int id)
        {
            if (id == 0)
            {

                return NotFound();
            }
            var brand = _unitOf.Brand.GetFirstOrDefault(x => x.Id == id);
            if (brand == null)
            {
                return NotFound();

            }
            return View(brand);
        }
        [HttpPost]
        public IActionResult Update(Brand brand)
        {
            // get brand name from db
            var brandnamefromdb = _unitOf.Brand.GetFirstOrDefault(p => p.Id == brand.Id).Name;
            if (!string.IsNullOrEmpty(brand.Name))
            {
                var dublicatebrand = _unitOf.Brand.GetFirstOrDefault(a => a.Name.ToLower() == brand.Name.ToLower());

                if (dublicatebrand != null && dublicatebrand.Name.ToLower() != brandnamefromdb.ToLower())
                {
                    // show message for duplicate brand nsme

                    //ModelState.AddModelError("Name", "the brand name is duplicated ");
                    ModelState.AddModelError(string.Empty, "the brand name is duplicated ");
                }
            }
            if (ModelState.IsValid)
            {
                _unitOf.Brand.ClearChangingTracking();
                _unitOf.Brand.  Update(brand);
                TempData.Add("success", "Brand   Updated Sucsusfully");
                _unitOf.Save();

                return RedirectToAction("Index");
            }
            return View(brand);
        }

        //  Delete
        [HttpGet]
        public IActionResult Delete(int id)
        {
            if (id == 0)
            {

                return NotFound();
            }
            var brand = _unitOf.Brand.GetFirstOrDefault(x => x.Id == id);
            if (brand == null)
            {
                return NotFound();

            }
            return View(brand);
        }
        [HttpPost, ActionName("Delete")]
        public IActionResult DeleteBrand(int id)
        {
            var brand = _unitOf.Brand.GetFirstOrDefault(p => p.Id == id);
            if (brand == null)
            {
                return NotFound();
            }

            _unitOf.Brand.Remove(brand);
            _unitOf.Save();
            TempData.Add("success", "Brand Deleted Sucsusfully");

            return RedirectToAction("Index");

        }



    }
}
