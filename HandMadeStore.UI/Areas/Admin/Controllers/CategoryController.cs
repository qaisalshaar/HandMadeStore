using AspNetCore.Reporting;
using HandMadeStore.DataAccess.Data;
using HandMadeStore.DataAccess.Repository;
using HandMadeStore.DataAccess.Repository.IRepository;
using HandMadeStore.Models.Models;
using HandMadeStore.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Globalization;

namespace HandMadeStore.UI.Areas.Admin.Controllers
{

    [Area("Admin")]
    [Authorize(Roles = SD.Role_Admin + "," + SD.Role_Moderator)]
    public class CategoryController : Controller
    {
        private readonly IUnitOfWork _unitOf;

        public CategoryController(IUnitOfWork unitOf)
        {
            _unitOf = unitOf;
        }

        public IActionResult Index()
        {
            IEnumerable<Category> categories = _unitOf.Category.GetAll();


            return View(categories);
        }


        // <i class="bi bi-plus-square"></i> &nbsp;  Create Category
        [HttpGet]
        public IActionResult Create()
        {


            return View();
        }
        [HttpPost]
        public IActionResult Create(Category category)
        {
            if (!string.IsNullOrEmpty(category.Name))
            {
                var dublicatecategory = _unitOf.Category.GetFirstOrDefault(a => a.Name.ToLower() == category.Name.ToLower());

                if (dublicatecategory != null)
                {
                    // show message for duplicate category name

                    //ModelState.AddModelError("Name", "the category name is duplicated ");
                    ModelState.AddModelError(string.Empty, "the category name is duplicated ");
                }


            }

            if (ModelState.IsValid)
            {
                _unitOf.Category.Add(category);
                _unitOf.Save();

                var IsArabic = CultureInfo.CurrentCulture.Name.StartsWith("ar");
                if (!IsArabic)
                {
                    TempData.Add("success", "Category Saved Sucsusfully");
                }
                else
                {
                    TempData.Add("success", "تم إضافة الصنف بنجاح");

                }

              
                return RedirectToAction("Index");

            }
            return View(category);
        }



        //   Update Category
        [HttpGet]
        public IActionResult   Update(int id)
        {
            if (id == 0)
            {

                return NotFound();
            }
            var category = _unitOf.Category.GetFirstOrDefault(x => x.Id == id);
            if (category == null)
            {
                return NotFound();

            }
            return View(category);
        }
        [HttpPost]
        public IActionResult   Update(Category category)
        {
            // get category name from db
            var categorynamefromdb = _unitOf.Category.GetFirstOrDefault(p => p.Id == category.Id).Name;
            if (!string.IsNullOrEmpty(category.Name))
            {
                var dublicatecategory = _unitOf.Category.GetFirstOrDefault(a => a.Name.ToLower() == category.Name.ToLower());

                if (dublicatecategory != null && dublicatecategory.Name.ToLower() != categorynamefromdb.ToLower())
                {
                    // show message for duplicate category nsme

                    //ModelState.AddModelError("Name", "the category name is duplicated ");
                    ModelState.AddModelError(string.Empty, "the category name is duplicated ");
                }
            }
            if (ModelState.IsValid)
            {
                _unitOf.Category.ClearChangingTracking();
                _unitOf.Category.  Update(category);
                var IsArabic = CultureInfo.CurrentCulture.Name.StartsWith("ar");
                if (!IsArabic)
                {
                    TempData.Add("success", "Category  Updated Sucsusfully");
                }
                else
                {
                    TempData.Add("success", "تم تعديل الصنف بنجاح");

                }

               
                _unitOf.Save();

                return RedirectToAction("Index");
            }
            return View(category);
        }

        //  Delete
        [HttpGet]
        public IActionResult Delete(int id)
        {
            if (id == 0)
            {

                return NotFound();
            }
            var category = _unitOf.Category.GetFirstOrDefault(x => x.Id == id);
            if (category == null)
            {
                return NotFound();

            }
            return View(category);
        }
        [HttpPost, ActionName("Delete")]
        public IActionResult DeleteCategory(int id)
        {
            var category = _unitOf.Category.GetFirstOrDefault(p => p.Id == id);
            if (category == null)
            {
                return NotFound();
            }

            _unitOf.Category.Remove(category);
            _unitOf.Save();

            var IsArabic = CultureInfo.CurrentCulture.Name.StartsWith("ar");
            if (!IsArabic)
            {
                TempData.Add("error", "Category Deleted Sucsusfully");
            }
            else
            {
                TempData.Add("error", "تم حذف الصنف بنجاح");

            }



           

            return RedirectToAction("Index");

        }

        public IActionResult PrintCategory()
        {
            IEnumerable<Category> categories = _unitOf.Category.GetAll();
            // للوصول للمسار الموجود فيه التقرير
            // bin folder
            var directory = AppContext.BaseDirectory;
            var path = Path.Combine(directory, "Categories.rdlc");
            LocalReport report=new LocalReport(path);
            report.AddDataSource("CategorySet", categories);

            var result = report.Execute(RenderType.Pdf);
          //  var result1 = report.Execute(RenderType.Word);

            
            return File(result.MainStream, "Application/pdf");


        }

    }
}
