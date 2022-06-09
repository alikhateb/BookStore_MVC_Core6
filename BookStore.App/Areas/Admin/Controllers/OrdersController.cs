using BookStore.DataAccess.UnitOfWork;
using BookStore.Models;
using BookStore.Models.ViewModels;
using BookStore.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BookStore.App.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize]
    public class OrdersController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        [BindProperty]
        public OrderVM OrderVM { get; set; }

        public OrdersController(IUnitOfWork _unitOfWork)
        {
            this._unitOfWork = _unitOfWork;
        }

        public IActionResult Index()
        {
            return View();
        }

        #region API Calls

        [HttpGet]
        public IActionResult GetAll(string status)
        {
            List<Order> orders;

            if (User.IsInRole(StaticDetails.Role_Admin) || User.IsInRole(StaticDetails.Role_Employee))
            {
                orders = _unitOfWork.Order.GetAll().OrderBy(x => x.Id).ToList();

                if (orders is null)
                {
                    return NotFound("no data found");
                }

                switch (status)
                {
                    case "approved":
                        orders = orders.Where(o => o.OrderStatus == StaticDetails.StatusApproved).ToList();
                        break;
                    case "pending":
                        orders = orders.Where(o => o.PaymentStatus == StaticDetails.PaymentStatusDelayedPayment).ToList();
                        break;
                    case "inprocess":
                        orders = orders.Where(o => o.OrderStatus == StaticDetails.StatusInProcess).ToList();
                        break;
                    case "completed":
                        orders = orders.Where(o => o.OrderStatus == StaticDetails.StatusShipped).ToList();
                        break;
                    default:
                        break;
                }
            }
            else
            {
                var claimsIdentity = User.Identity as ClaimsIdentity;
                var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

                orders = _unitOfWork.Order.GetAll(o => o.AppUserId == claim.Value).ToList();

                if (orders is null)
                {
                    return NotFound("no data found");
                }
            }

            return Json(new { data = orders });
        }

        #endregion

        public IActionResult Details(int orderId)
        {
            OrderVM = new()
            {
                Order = _unitOfWork.Order.FindObject(order => order.Id == orderId),
                OrderDetails = _unitOfWork.OrderDetail.GetAll(o => o.OrderId == orderId, x => x.Product).ToList(),
            };

            if (OrderVM.Order is null)
            {
                return NotFound("no data found");
            }

            return View(OrderVM);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult UpdateOrderDetails()
        {
            var order = _unitOfWork.Order.FindObject(o => o.Id == OrderVM.Order.Id);
            //if (!ModelState.IsValid)
            //{
            //    OrderVM.Order = order;
            //    OrderVM.OrderDetails = _unitOfWork.OrderDetail.GetAll(o => o.OrderId == OrderVM.Order.Id, x => x.Product).ToList();
            //    return View(nameof(Details), OrderVM);
            //}

            order.Username = OrderVM.Order.Username;
            order.PhoneNumber = OrderVM.Order.PhoneNumber;
            order.Address = OrderVM.Order.Address;
            order.City = OrderVM.Order.City;
            order.State = OrderVM.Order.State;
            order.PostalCode = OrderVM.Order.PostalCode;
            if (OrderVM.Order.Carrier is not null)
            {
                order.Carrier = OrderVM.Order.Carrier;
            }
            if (OrderVM.Order.TrackingNumber is not null)
            {
                order.TrackingNumber = OrderVM.Order.TrackingNumber;
            }

            _unitOfWork.Order.Update(order);
            _unitOfWork.Save();
            return RedirectToAction(nameof(Details), new { orderId = order.Id });
        }
    }
}
