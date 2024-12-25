using HandMadeStore.DataAccess.Repository.IRepository;
using HandMadeStore.Models.Models;
using HandMadeStore.Models.Models.ViewModels;
using HandMadeStore.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Stripe.Checkout;
using System.Drawing.Imaging;
using System.Drawing;
using System.Security.Claims;
using System;

using System.Windows.Forms;
namespace HandMadeStore.UI.Areas.Customer.Controllers
{

    [Area("Customer")]
    [Authorize]

    public class CartController : Controller
    {
        private readonly IUnitOfWork _unitOf;
        private readonly IEmailSender _emailSender;
        private readonly IWebHostEnvironment _hostEnvironment;
        private readonly UserManager<IdentityUser> _userManager;

        [BindProperty]
        public CartVM cartvm { get; set; }
        //public OrderHeader orderheader { get; set; }
        public CartController(IUnitOfWork unitOf, IEmailSender emailSender, IWebHostEnvironment hostEnvironment, UserManager<IdentityUser> userManager)
        {
            _unitOf = unitOf;
            _emailSender = emailSender;
            _hostEnvironment = hostEnvironment;
            _userManager = userManager;
        }


        //[AllowAnonymous]
        // تستخدم في حال الغاء عمل ال Authorise 
        // على ميثود معينة
        public IActionResult Index()
        {

            // الحصول على اسم المستخدم

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            cartvm = new CartVM()
            {
                CartItems = _unitOf.CartItem.GetAll(c => c.ApplicationUserId == userId, "Product")


            };

            // تعديل السعر بحسب العدد
            foreach (var item in cartvm.CartItems)
            {
                item.PriceFromCount = GetPrice(item.ProductCount, item.Product.Price, item.Product.Price10plus, item.Product.Price30plus);
                // الحصول على سعر اجمالي قيمة سلة المشتريات 
                cartvm.CartTotal += (item.PriceFromCount * item.ProductCount);
                // اجمالي عدد القطع في سلة المشتريات
                cartvm.ItemPeicesCount += item.ProductCount;
            }


            return View(cartvm);
        }




        // Summary cart GET
        public IActionResult Summary()
        {

            //// الحصول على اسم المستخدم

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);


            // Appuser = _unitOf.ApplicationUser.GetFirstOrDefault(c => c.Id == userId);

            cartvm = new CartVM()
            {
                CartItems = _unitOf.CartItem.GetAll(c => c.ApplicationUserId == userId, "Product"),
                orderheader = new()


            };

            // OrderHeader
            cartvm.orderheader.ApplicationUser = _unitOf.ApplicationUser.GetFirstOrDefault(c => c.Id == userId);


            cartvm.orderheader.Name = cartvm.orderheader.ApplicationUser.Name;
            cartvm.orderheader.PhoneNumber = cartvm.orderheader.ApplicationUser.PhoneNumber;
            cartvm.orderheader.StreetAddress = cartvm.orderheader.ApplicationUser.StreetName;
            cartvm.orderheader.City = cartvm.orderheader.ApplicationUser.City;
            cartvm.orderheader.PostalCode = cartvm.orderheader.ApplicationUser.PostalCode;



            // تعديل السعر بحسب العدد
            foreach (var item in cartvm.CartItems)
            {
                item.PriceFromCount = GetPrice(item.ProductCount, item.Product.Price, item.Product.Price10plus, item.Product.Price30plus);
                // الحصول على سعر اجمالي قيمة سلة المشتريات 
                cartvm.CartTotal += (item.PriceFromCount * item.ProductCount);
                // الحصول على سعر اجمالي قيمة سلة المشتريات 
                cartvm.orderheader.OrderTotal += (item.PriceFromCount * item.ProductCount);
                // اجمالي عدد القطع في سلة المشتريات
                cartvm.ItemPeicesCount += item.ProductCount;
            }





            return View(cartvm);
        }






        // Summary cart Post
        //or
        //public IActionResult SummaryPost(CartVM cartvm)
        // then   [BindProperty] in above
        //   public CartVM cartvm { get; set; }
        [HttpPost]
        [ActionName("Summary")]
        public IActionResult SummaryPost()

        {

            //// الحصول على اسم المستخدم

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            // get user email
            var useremail = _unitOf.ApplicationUser.GetFirstOrDefault(c => c.Id == userId).Email;

            cartvm.CartItems = _unitOf.CartItem.GetAll(c => c.ApplicationUserId == userId, "Product");

            // التحقق من اليوزر انه user or shop
            // get applicationuser

            ApplicationUser applicationUser = _unitOf.ApplicationUser.GetFirstOrDefault(c => c.Id == userId);

            if (applicationUser.ShopId.GetValueOrDefault() == 0)
            {

                // customer

                cartvm.orderheader.OrderStatus = SD.StatusPending;
                cartvm.orderheader.PaymentStatus = SD.StatusPending;

            }
            else
            {
                // shop
                cartvm.orderheader.OrderStatus = SD.StatusApproved;
                cartvm.orderheader.PaymentStatus = SD.PaymentStatusDelayedPayment;

            }
            cartvm.orderheader.ApplicationUserId = userId;

            cartvm.orderheader.OrderDate = DateTime.Now;










            foreach (var item in cartvm.CartItems)
            {  // تعديل السعر بحسب العدد
                item.PriceFromCount = GetPrice(item.ProductCount, item.Product.Price, item.Product.Price10plus, item.Product.Price30plus);
                // الحصول على سعر اجمالي قيمة سلة المشتريات 
                cartvm.CartTotal += (item.PriceFromCount * item.ProductCount);
                // الحصول على سعر اجمالي قيمة سلة المشتريات 
                cartvm.orderheader.OrderTotal += (item.PriceFromCount * item.ProductCount);
                // اجمالي عدد القطع في سلة المشتريات
                cartvm.ItemPeicesCount += item.ProductCount;
            }



            // save items in order Header table

            _unitOf.OrderHeader.Add(cartvm.orderheader);
            _unitOf.Save();

            // Save items in order Details table

            foreach (var item in cartvm.CartItems)
            {
                OrderDetail orderdetail = new OrderDetail()
                {

                    ProductId = item.productId,
                    OrderId = cartvm.orderheader.Id,
                    Price = item.PriceFromCount,
                    Count = item.ProductCount,


                };
                _unitOf.OrderDetail.Add(orderdetail);
                _unitOf.Save();
            }

            //بداية اضافة Stripe payment

            if (applicationUser.ShopId.GetValueOrDefault() == 0)
            {
                // customer

                var options = new SessionCreateOptions
                {
                    // cartitems in shoping card
                    LineItems = new List<SessionLineItemOptions>()  ,
                  
     
                    // تحديد نظام السيشن
                    // هل المود هو سداد او دفع او اشتراك 
                    Mode = "payment",

                    
                    // طريقة السداد او الدفع 
                    PaymentMethodTypes = new List<string>
                {
                   // في عندنا اكثر من طريقة دفع 
                   // Card (visa or mastercard)
                   // walet (محفظة الكترونية)
                   // Bank Transfer
                   // وفي المشروع الذي معنا سنستخدم ال card
                   "card"

                },

                    CustomerEmail = useremail,
                  

                    // الحصول على الدومين الخاص بالموقع
                    SuccessUrl = SD.AppUrlDomain + $"Customer/cart/OrderConformation?id={cartvm.orderheader.Id}",
                    CancelUrl = SD.AppUrlDomain + $"Customer/cart/Index"
                };


                foreach (var item in cartvm.CartItems)
                {

                    {
                        var setionLineItem = new SessionLineItemOptions
                        {
                            PriceData = new SessionLineItemPriceDataOptions
                            {
                                UnitAmount = (long)(item.PriceFromCount * 100),
                                Currency = "usd",
                                ProductData = new SessionLineItemPriceDataProductDataOptions
                                {
                                    Name = item.Product.Name,
                                },
                            },
                            Quantity = item.ProductCount,


                        };

                        options.LineItems.Add(setionLineItem);
                    }

                }


                var service = new SessionService();
                Session session = service.Create(options);

                // update Orderheader Payment values
                _unitOf.OrderHeader.UpdateOrderPayment(cartvm.orderheader.Id, session.Id, session.PaymentIntentId);
                _unitOf.Save();
                Response.Headers.Add("Location", session.Url);
                return new StatusCodeResult(303);


            }
            else
            {

                // Shop
                // لان السداد او الدفع  بعد 30 يوم عند الشحن
                return RedirectToAction("OrderConformation", "cart", new { id = cartvm.orderheader.Id });
            }





            // نهاية اضافة stripe payment


            // حذف cartitem  الخاصة بالمستخدم لانه تم حفظها في الجدول
            // اي تفريغ سلة المشتريات للمستخدم
            // حذف اكثر من سجل

            //_unitOf.CartItem.RemoveRange(cartvm.CartItems);
            //_unitOf.Save();
            //return RedirectToAction("Index","Home");
        }

        // add Coformation method
        // id == OrderheaderId

        public  IActionResult OrderConformation(int id, string returnUrl = null)
        {

            var orderheader =  _unitOf.OrderHeader.GetFirstOrDefault(o => o.Id == id,"ApplicationUser");

            // التحقق من ان المستخدم customer  or Shop
            if (orderheader.PaymentStatus != SD.PaymentStatusDelayedPayment)
            {
                // customer
                // الحصول على السشن المفتوحة مع ال stripe
                var service = new SessionService();
                Session session = service.Get(orderheader.SessionId);
                // التحقق من payment status
                // اذا كانت paid 
                //  approve فيتم تعديل الحالة الى 

                if (session.PaymentStatus.ToLower() == "paid")
                {
                    // تحديث بيانات
                    _unitOf.OrderHeader.UpdateOrderPayment(orderheader.Id, session.Id, session.PaymentIntentId);

                    _unitOf.OrderHeader.UpdateStatus(id, SD.StatusApproved, SD.PaymentStatusApproved);
                    _unitOf.Save();
                    // add to CartSession

                    HttpContext.Session.SetInt32(SD.CartSession, 0);

                }

            }
            // send email to User
           // _emailSender.SendEmailAsync(orderheader.ApplicationUser.Email, "New Order - Handmade Store", $"<h2>Order #{orderheader.Id} Created Successfully ... Total : {orderheader.OrderTotal} $</h2>");



           


             _emailSender.SendEmailAsync(orderheader.ApplicationUser.Email, "New Order - Handmade Store", $"<h2>Order #{orderheader.Id} Created Successfully ... Total : {orderheader.OrderTotal} $</h2>");


            // end Order Conformation





            //else  shop
            // حذف cartitem  الخاصة بالمستخدم لانه تم حفظها في الجدول
            // اي تفريغ سلة المشتريات للمستخدم
            // حذف اكثر من سجل
            List<CartItem> cartitems = _unitOf.CartItem.GetAll(c => c.ApplicationUserId == orderheader.ApplicationUserId).ToList();
            _unitOf.CartItem.RemoveRange(cartitems);
            _unitOf.Save();
            return View(id);

        }



        //private void CaptureMyScreen()
        //{
        //    try
        //    {
        //        //Creating a new Bitmap object
        //        Bitmap captureBitmap = new Bitmap(1024, 768, PixelFormat.Format32bppArgb);
        //        //Bitmap captureBitmap = new Bitmap(int width, int height, PixelFormat);
        //        //Creating a Rectangle object which will
        //        //capture our Current Screen
        //        Rectangle captureRectangle = Screen.AllScreens[0].Bounds;
        //        //Creating a New Graphics Object
        //        Graphics captureGraphics = Graphics.FromImage(captureBitmap);
        //        //Copying Image from The Screen
        //        captureGraphics.CopyFromScreen(captureRectangle.Left, captureRectangle.Top, 0, 0, captureRectangle.Size);
        //        //Saving the Image File (I am here Saving it in My E drive).
        //        captureBitmap.Save(@"E:\Capture.jpg", ImageFormat.Jpeg);
        //        //Displaying the Successfull Result
        //     //   MessageBox.Show("Screen Captured");
        //    }
        //    catch (Exception ex)
        //    {
        //       // MessageBox.Show(ex.Message);
        //    }
        //}








        // زيادة عدد المنتج المشتراه بمقدار 1
        public IActionResult Increment(int cartid)
        {
            var cartitem = _unitOf.CartItem.GetFirstOrDefault(c => c.Id == cartid);
            _unitOf.CartItem.IncrementCount(cartitem, 1);
            _unitOf.Save();

            // add to CartSession

            HttpContext.Session.SetInt32(SD.CartSession, _unitOf.CartItem.GetPiecesCount());
            return RedirectToAction("Index");
        }
        // نقصان عدد المنتج المشتراه بمقدار 1
        public IActionResult Decrement(int cartid)
        {
            var cartitem = _unitOf.CartItem.GetFirstOrDefault(c => c.Id == cartid);

            if (cartitem.ProductCount <= 1)
            {

                // remove item from cart items
                _unitOf.CartItem.Remove(cartitem);
            }
            else
            {
                _unitOf.CartItem.DcrementCount(cartitem, 1);

            }

            _unitOf.Save();
            // add to CartSession

            HttpContext.Session.SetInt32(SD.CartSession, _unitOf.CartItem.GetPiecesCount());
            return RedirectToAction("Index");
        }



        // حذف المنتج
        public IActionResult RemoveFromCart(int cartid)
        {
            var cartitem = _unitOf.CartItem.GetFirstOrDefault(c => c.Id == cartid);
            _unitOf.CartItem.Remove(cartitem);
            _unitOf.Save();

            // add to CartSession

            HttpContext.Session.SetInt32(SD.CartSession, _unitOf.CartItem.GetPiecesCount());
            return RedirectToAction("Index");
        }



        // حساب السعر بحسب العدد

        private double GetPrice(int count, double? price, double? price10plus, double? price30plus)
        {

            if (count <= 10)
            {
                return (Double)price;

            }
            else
            {
                if (count <= 30)
                {
                    return (Double)price10plus;

                }
                else
                {

                    return (Double)price30plus;
                }

            }

        }

    }
}

