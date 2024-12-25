using HandMadeStore.DataAccess.Repository;
using HandMadeStore.DataAccess.Repository.IRepository;
using HandMadeStore.Models.Models;
using HandMadeStore.Models.Models.ViewModels;
using HandMadeStore.UI.Hubs;
using HandMadeStore.UI.Models;
using HandMadeStore.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.SignalR;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Options;
using Stripe;
using System.Diagnostics;
using System.Security.Claims;


//using Product = HandMadeStore.Models.Models.Product;


namespace HandMadeStore.UI.Areas.Customer.Controllers
{

    [Area("Customer")]
    public class HomeController : Controller
    {

        private readonly ILogger<HomeController> _logger;
        private readonly IEmailSender _emailSender;
        private readonly IHubContext<ReviewsHub> _reviewshub;
        private readonly IHubContext<MessageHub> _messagehub;
        private readonly IEmailSenderNew _iemailsendernew;
        private readonly IStringLocalizer<HomeController> _locolaizer;

        //[BindProperty]
        //public HandMadeStore.Models.Models.ReviewCustomer ReviewCustomer { get; set; }

        public IUnitOfWork _unitOf { get; }

        public HomeController(ILogger<HomeController> logger, IUnitOfWork unitOf, IEmailSender emailSender, IHubContext<ReviewsHub> reviewshub, IHubContext<MessageHub> messagehub, IEmailSenderNew iemailsendernew,
            IStringLocalizer<HomeController> locolaizer)
        {
            _logger = logger;
            _unitOf = unitOf;
            _emailSender = emailSender;
            _reviewshub = reviewshub;
            _messagehub = messagehub;
            _iemailsendernew = iemailsendernew;
            _locolaizer = locolaizer;
        }
        [HttpGet]
        public IActionResult Index(string proname)
        {
          //  ViewBag.welcomemessage = _locolaizer["welcome"];
           // ViewBag.welcomemessage = String.Format(   _locolaizer["welcome"],"Kais Alshaar","HandMade Store");

            // get GetPiecesCount

            HttpContext.Session.SetInt32(SD.CartSession, _unitOf.CartItem.GetPiecesCount());

            if (proname == null)
            {
                List<HandMadeStore.Models.Models.Product> product = _unitOf.Product.GetAll(inclodePropertiies: "Category,Brand").ToList();

                //IEnumerable<Product> product1=_unitOf.Product.GetAll();
                return View(product);
            }
            else
            {
                List<HandMadeStore.Models.Models.Product> producttems = _unitOf.Product.GetAll(c => c.Name.Contains(proname) || proname == null || c.Category.Name.Contains(proname) || c.Description.Contains(proname), inclodePropertiies: "Category,Brand").ToList();
                //var checkproductname = _unitOf.Product.GetFirstOrDefault(p => p.Name == proname).Name;


                //return Json(1);
                // return Json(new { data = producttems });
                return View(producttems);

            }




        }
        // GET
        public IActionResult Details(int proid)
        {
            CartItem cartitem = new()
            {

                ProductCount = 1,
                productId = proid,
                Product = _unitOf.Product.GetFirstOrDefault(p => p.Id == proid, inclodePropertiies: "Category,Brand")


            };
            // for review
            TempData["product_id"] = proid;

            // جلب بيانات الرفيورز
            IEnumerable<ReviewCustomer> reviews = _unitOf.ReviewCustomer.GetAll(r => r.ProductId == proid, inclodePropertiies: "ApplicationUser");
            // تخزين بيامات الرفيوز
            ViewData["Reviews"] = reviews;


            return View(cartitem);
        }
        // Post
        // add to cart item
        // يجب التاكد من ان المستخدم قد تم الدخول الى الموقع
        [HttpPost]
        [Authorize]
        public IActionResult Details(CartItem cartitem)
        {
            // الحصول على اسم المستخدم

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            cartitem.ApplicationUserId = userId;


            // التحقق من وجود نفس الصنف في ال cartitem 

            var cartitemfromdb = _unitOf.CartItem.GetFirstOrDefault(c => c.ApplicationUserId == cartitem.ApplicationUserId && c.productId == cartitem.productId);
            if (cartitemfromdb == null)
            {

                _unitOf.CartItem.Add(cartitem);
            }
            else
            {
                _unitOf.CartItem.IncrementCount(cartitemfromdb, cartitem.ProductCount);

            }



            _unitOf.Save();
            // add to CartSession

            HttpContext.Session.SetInt32(SD.CartSession, _unitOf.CartItem.GetPiecesCount());
            // return RedirectToAction("Index");
            // or
            return RedirectToAction(nameof(Index));

        }


        [HttpPost]
        // returnurl  معرفة المكان الي واقف عليه للرجوع الى نفس الصفحة في حال تغيير اللغة
        public IActionResult SetLanguage(string culture,string returnurl)
        {

            Response.Cookies.Append(
                
                CookieRequestCultureProvider.DefaultCookieName,
                CookieRequestCultureProvider.MakeCookieValue(requestCulture:new RequestCulture(culture)),
                 new CookieOptions { Expires = DateTimeOffset.UtcNow.AddYears(years:1)}
                
                
                );
            return LocalRedirect(returnurl);
        }


        public IActionResult SetCulture1(string lang, string returnurl)
        {

            Response.Cookies.Append(CookieRequestCultureProvider.DefaultCookieName,
       CookieRequestCultureProvider.MakeCookieValue(new RequestCulture(lang)),
       new CookieOptions { Expires = DateTimeOffset.UtcNow.AddYears(years: 1) });

            //return RedirectToAction("Index");
            // لتوجيهنا الى المكان الذي كنا فيه
            return LocalRedirect(returnurl);
        }


        [HttpPost]
        [Authorize]
        public IActionResult AddReview(ReviewCustomer reviewcustomer)
        {


            if (ModelState.IsValid)
            {

                if (!string.IsNullOrEmpty(reviewcustomer.ReviewText))
                {

                    var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                    reviewcustomer.ApplicationUserId = userId;

                    reviewcustomer.ProductId = (int)TempData["product_id"];
                    reviewcustomer.ReviewDate = DateTime.Now;
                    _unitOf.ReviewCustomer.Add(reviewcustomer);
                    // send Signalr Message to all clients

                    _reviewshub.Clients.All.SendAsync("loadReviews", reviewcustomer.ProductId);



                    _unitOf.Save();
                    // للرجوع الى نفس الصفحة
                    return RedirectToAction("Details", "Home", new { proid = reviewcustomer.ProductId });

                }

            }
            //  ModelState.AddModelError(string.Empty, "the Review is Empty");
            // للرجوع الى نفس الصفحة

            return RedirectToAction("Details", "Home", new { proid = reviewcustomer.ProductId });


        }



        public IActionResult About()
        {
            return View();
        }

        public IActionResult Contact()
        {

            return View();
        }

        public IActionResult Contact1()
        {

            return View();
        }


        [Authorize]
        [HttpPost]
        public async Task<IActionResult> SendContact1(string rec, string sub, string mes)
        {

            var mestest = System.Web.HttpUtility.HtmlEncode("which is <h1>not</h1> included in the Quote");
            await _iemailsendernew.SendEmailAsync(rec, sub, Environment.NewLine + "<div style='-webkit-text-stroke-width:0px;color:#242424;font-family:&quot;Times New Roman&quot;; font-size:medium; font-style:normal; font-variant-caps:normal; font-variant-ligatures:normal; font-weight:400; letter-spacing:normal; orphans:2; text-align:start; text-decoration-color:initial; text-decoration-style:initial; text-decoration-thickness:initial; text-indent:0px; text-transform:none; white-space:normal; widows:2; word-spacing:0px'>Note this is an automated message. If you have any questions, please reach out to the CRM Team " + rec + "</div></div>");
            return RedirectToAction("Contact");
        }
            
        [Authorize]
        [HttpPost]
        public IActionResult Send(MessageVM messagevm)
        {
            // الحصول على رقم المستخدم الحالي
            var userid = User.FindFirstValue(ClaimTypes.NameIdentifier);
            //  او الايميل الحصول على اسم المستخدم الذي ارسل الرسالة
            var Sender = _unitOf.ApplicationUser.GetFirstOrDefault(x => x.Id == userid);


            // والان نريد الحصول على الادمن لارسال رسالة له
            var Reciever = _unitOf.ApplicationUser.GetFirstOrDefault(x => x.UserName == "qaisalshaar2020@outlook.com");


            var MessageToSend = new
            {

                Sender = Sender.Name,
                Body = messagevm.MessageText



            };

            _messagehub.Clients.User(Reciever.Id).SendAsync("recieveMessage", MessageToSend);


            return RedirectToAction("Contact");
        }





        [Authorize]
        [HttpPost]
        public IActionResult Contact(string mailto, string subject, string messageBody)
        {

            ViewBag.mailto = string.Format("Name:{0}", mailto).Trim();
            ViewBag.subject = string.Format("Name:{0}", subject);
            ViewBag.messageBody = string.Format("Name:{0}", messageBody);

            //   _emailSender.SendEmailAsync(string.Format("Name:{0}", mailto), string.Format("Name:{0}", subject), string.Format("Name:{0}", messageBody));


            _emailSender.SendEmailAsync(mailto, subject, messageBody);


            _emailSender.SendEmailAsync(mailto, $"New E-Mail From - Handmade Store Customer:{subject}", $"<h2>{messageBody}</h2>");

            return View();
        }


        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }





        //SearchOption test




        [HttpGet]
        public JsonResult SearchbyProductname(string proname200)

        {

            if (proname200 != null)
            {
                List<HandMadeStore.Models.Models.Product> producttems = _unitOf.Product.GetAll(c => c.Name.Contains(proname200) || proname200 == null || c.Category.Name.Contains(proname200) || c.Description.Contains(proname200), inclodePropertiies: "Category,Brand").ToList();
                //var checkproductname = _unitOf.Product.GetFirstOrDefault(p => p.Name == proname).Name;


                if (producttems.Count == 0)
                {
                    //  return Json(new { resp = "undefined".ToString() }) ;
                    return Json("");
                }
                else
                {
                    //  return Json(new { resp = producttems });
                    return Json(producttems);
                }
                //return Json(1);
                // return Json(new { data = producttems });

            }
            else
            {
                return Json("");
            }




        }



    }


}
