﻿
using HandMadeStore.DataAccess.Repository.IRepository;
using HandMadeStore.Models.Models;
using HandMadeStore.Models.Models.ViewModels;
using HandMadeStore.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Stripe;
using Stripe.Checkout;
using System.Diagnostics;
using System.Security.Claims;

namespace HandmadeStore.UI.Areas.Admin.Controllers
{
    [Area("Admin")]
  //  [Authorize(Roles = SD.Role_Admin + "," + SD.Role_Moderator)]
    [Authorize]
    public class OrderController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
       [BindProperty]
        public OrderVM orderVM { get; set; }

        public OrderController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public IActionResult Index()
        {
            return View();
        }

       
        public IActionResult Details(int orderid)
        {

            orderVM= new OrderVM()
            {

                OrderHeader=_unitOfWork.OrderHeader.GetFirstOrDefault(o=>o.Id== orderid, "ApplicationUser"),

                


                OrderDetails = _unitOfWork.OrderDetail.GetAll(o => o.OrderId == orderid, "Product"),
            };
            // تحديد رقم التتبع للشحن من خلال اضافة رقم الطلب مع التاريخ الحالي
            orderVM.OrderHeader.TrackingNumber = orderVM.OrderHeader.Id +"-"+ DateTime.Now.ToString("dd-MM-yyyy");
            _unitOfWork.OrderHeader.Update(orderVM.OrderHeader);

            _unitOfWork.Save();

            return View(orderVM);
        }
        //  [HttpPost, ActionName("UpdateOrder")]
        [HttpPost]
        [Authorize(Roles = SD.Role_Admin + "," + SD.Role_Moderator)]
        public IActionResult UpdateOrderDetails()
        {
            var orderHeaderFromDb = _unitOfWork.OrderHeader.GetFirstOrDefault(u => u.Id == orderVM.OrderHeader.Id);
            orderHeaderFromDb.Name = orderVM.OrderHeader.Name;
            orderHeaderFromDb.PhoneNumber = orderVM.OrderHeader.PhoneNumber;
            orderHeaderFromDb.StreetAddress = orderVM.OrderHeader.StreetAddress;
            orderHeaderFromDb.City = orderVM.OrderHeader.City;
            if (orderVM.OrderHeader.Carrier != null)
            {
                orderHeaderFromDb.Carrier = orderVM.OrderHeader.Carrier;
            }
            if (orderVM.OrderHeader.TrackingNumber != null)
            {
                orderHeaderFromDb.TrackingNumber = orderVM.OrderHeader.TrackingNumber;
            }
            _unitOfWork.OrderHeader.Update(orderHeaderFromDb);
          
            _unitOfWork.Save();
            TempData["Success"] = "Order Details Updated Successfully.";
            //الرجوع الى نفس الصفحة بعد التحديث
            return RedirectToAction("Details", "Order", new { orderId = orderHeaderFromDb.Id });
        }


        [HttpPost]
        [Authorize(Roles = SD.Role_Admin + "," + SD.Role_Moderator)]
        public IActionResult StartProcessing()
        {
            _unitOfWork.OrderHeader.UpdateStatus(orderVM.OrderHeader.Id, SD.StatusInProcess);
            _unitOfWork.Save();
            TempData["Success"] = "Order Status Updated Successfully.";
            //الرجوع الى نفس الصفحة بعد التحديث
            return RedirectToAction("Details", "Order", new { orderId = orderVM.OrderHeader.Id });
        }

        [HttpPost]
        [Authorize(Roles = SD.Role_Admin + "," + SD.Role_Moderator)]
        public IActionResult ShipOrder()
        {
            //يجب ادخال شركة الشحن وتتبع الشحن
            var orderHeader = _unitOfWork.OrderHeader.GetFirstOrDefault(u => u.Id == orderVM.OrderHeader.Id);
            orderHeader.Carrier = orderVM.OrderHeader.Carrier;
            orderHeader.TrackingNumber = orderVM.OrderHeader.TrackingNumber;
            orderHeader.OrderStatus = SD.StatusShipped;
            orderHeader.ShippingDate = DateTime.Now;
            //في حال كان المستخدم محل  shop
            if (orderHeader.PaymentStatus == SD.PaymentStatusDelayedPayment)
            {
                //تاريخ استحقاق الدفع بعد ثلاثون يوما من الشحن
                orderHeader.PaymentDueDate = DateTime.Now.AddDays(30);
            }
            _unitOfWork.OrderHeader.Update(orderHeader);
            _unitOfWork.Save();
            TempData["Success"] = "Order Shipped Successfully.";
            //الرجوع الى نفس الصفحة بعد التحديث
            return RedirectToAction("Details", "Order", new { orderId = orderVM.OrderHeader.Id });
        }




        [HttpPost]
        [Authorize(Roles = SD.Role_Admin + "," + SD.Role_Moderator)]
        public IActionResult CancelOrder()
        {
            var orderHeader = _unitOfWork.OrderHeader.GetFirstOrDefault(o => o.Id == orderVM.OrderHeader.Id);
            if (orderHeader.PaymentStatus == SD.PaymentStatusApproved)
            {
                //Order is paid
                var options = new RefundCreateOptions
                {
                    Reason = RefundReasons.RequestedByCustomer,
                    PaymentIntent = orderHeader.PaymentIntentId
                    
                };

                var service = new RefundService();
                Refund refund = service.Create(options);

                _unitOfWork.OrderHeader.UpdateStatus(orderHeader.Id, SD.StatusCancelled, SD.StatusRefunded);
            }
            //Order still not paid
            else
            {
                _unitOfWork.OrderHeader.UpdateStatus(orderHeader.Id, SD.StatusCancelled, SD.StatusCancelled);
            }
            _unitOfWork.Save();

            TempData["Success"] = "Order Cancelled Successfully.";
            return RedirectToAction("Details", "Order", new { orderId = orderVM.OrderHeader.Id });
        }






        [HttpPost]
        [Authorize(Roles = SD.Role_Admin + "," + SD.Role_Moderator)]
        public IActionResult PayNow()
        {
            orderVM.OrderHeader = _unitOfWork.OrderHeader.GetFirstOrDefault(o => o.Id == orderVM.OrderHeader.Id, inclodePropertiies: "ApplicationUser");
            orderVM.OrderDetails = _unitOfWork.OrderDetail.GetAll(o => o.OrderId == orderVM.OrderHeader.Id, inclodePropertiies: "Product");
            //// الحصول على اسم المستخدم

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            // get user email
            var useremail = _unitOfWork.ApplicationUser.GetFirstOrDefault(c => c.Id == userId).Email;
            //stripe settings 
            var domain = SD.AppUrlDomain;
            var options = new SessionCreateOptions
            {
                PaymentMethodTypes = new List<string>
                {
                  "card"
                },
                LineItems = new List<SessionLineItemOptions>(),
                Mode = "payment",
                SuccessUrl = domain + $"admin/order/PaymentConfirmation?orderHeaderId={orderVM.OrderHeader.Id}",
                CancelUrl = domain + $"admin/order/details?orderId={orderVM.OrderHeader.Id}",
                CustomerEmail = useremail,
            };

            foreach (var item in orderVM.OrderDetails)
            {

                var sessionLineItem = new SessionLineItemOptions
                {
                    PriceData = new SessionLineItemPriceDataOptions
                    {
                        UnitAmount = (long)(item.Price * 100),//20.00 -> 2000
                        Currency = "usd",
                        ProductData = new SessionLineItemPriceDataProductDataOptions
                        {
                            Name = item.Product.Name
                        }
                    },
                    Quantity = item.Count,
                };
                options.LineItems.Add(sessionLineItem);
            }

            var service = new SessionService();
            Session session = service.Create(options);
            _unitOfWork.OrderHeader.UpdateOrderPayment(orderVM.OrderHeader.Id, session.Id, session.PaymentIntentId);
            _unitOfWork.Save();
            Response.Headers.Add("Location", session.Url);
            // التوجه الى صفحة الدفع
            return new StatusCodeResult(303);
        }

        public IActionResult PaymentConfirmation(int orderHeaderId)
        {
            OrderHeader orderHeader = _unitOfWork.OrderHeader.GetFirstOrDefault(o => o.Id == orderHeaderId);
            if (orderHeader.PaymentStatus == SD.PaymentStatusDelayedPayment)
            {
                var service = new SessionService();
                Session session = service.Get(orderHeader.SessionId);
                //check the stripe status
                if (session.PaymentStatus.ToLower() == "paid")
                {
                    _unitOfWork.OrderHeader.UpdateOrderPayment(orderHeader.Id, session.Id, session.PaymentIntentId);
                    _unitOfWork.OrderHeader.UpdateStatus(orderHeaderId, orderHeader.OrderStatus, SD.PaymentStatusApproved);
                    _unitOfWork.Save();
                }
            }
            return View(orderHeaderId);
        }


        #region API Endpoints
        //Get All Products Endpoint
        [HttpGet]
        public IActionResult GetAll( string status)
        {


            IEnumerable<OrderHeader> orderHeaders;

            // التحقق من جلب جميع البيانات في حال كان مدير او مشرف
            if(User.IsInRole(SD.Role_Admin) || User.IsInRole(SD.Role_Moderator))
            {
                // جلب جميع البيانات
                orderHeaders = _unitOfWork.OrderHeader.GetAll(inclodePropertiies: "ApplicationUser");

            }
            else
            {

                // جلب بيانات  customer or shop
                // getall with filter
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

                orderHeaders = _unitOfWork.OrderHeader.GetAll(o=>o.ApplicationUserId==userId, "ApplicationUser");


            }
            
        


            switch (status)
            {
                case "ppending":
                    orderHeaders = orderHeaders.Where(o => o.PaymentStatus == SD.PaymentStatusDelayedPayment);
                    break;
                case "cancel":
                    orderHeaders = orderHeaders.Where(o => o.OrderStatus == SD.StatusCancelled);
                    break;
                //case "ship":
                //    orderHeaders = orderHeaders.Where(o => o.OrderStatus == SD.StatusShipped);
                //    break;
                case "approved":
                    orderHeaders = orderHeaders.Where(o => o.OrderStatus == SD.StatusApproved);
                    break;
                case "inprocess":
                    orderHeaders = orderHeaders.Where(o => o.OrderStatus == SD.StatusInProcess);
                    break;
                case "completed":
                    orderHeaders = orderHeaders.Where(o => o.OrderStatus == SD.StatusShipped);
                    break;
                default:
                  
                    break;
            }




            return Json(new { data = orderHeaders });
        }
        #endregion
    }
}