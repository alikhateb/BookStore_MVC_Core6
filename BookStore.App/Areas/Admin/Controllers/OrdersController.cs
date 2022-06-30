using Stripe;

namespace BookStore.App.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = $"{StaticDetails.Role_Admin}, {StaticDetails.Role_Employee}")]
    public class OrdersController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        [BindProperty]
        public OrderVM OrderVM { get; set; }

#pragma warning disable CS8618 // Non-nullable property 'OrderVM' must contain a non-null value when exiting constructor. Consider declaring the property as nullable.
        public OrdersController(IUnitOfWork _unitOfWork)
#pragma warning restore CS8618 // Non-nullable property 'OrderVM' must contain a non-null value when exiting constructor. Consider declaring the property as nullable.
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
            List<Models.Models.Order> orders;

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
#pragma warning disable CS8602 // Dereference of a possibly null reference.
                var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
#pragma warning restore CS8602 // Dereference of a possibly null reference.

#pragma warning disable CS8602 // Dereference of a possibly null reference.
                orders = _unitOfWork.Order.GetAll(o => o.AppUserId == claim.Value).OrderBy(x => x.Id).ToList();
#pragma warning restore CS8602 // Dereference of a possibly null reference.

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
                //OrderDetails = _unitOfWork.OrderDetail.GetAll(o => o.OrderId == orderId, x => x.Product).ToList(),
                OrderDetails = _unitOfWork.OrderDetail.GetAll(o => o.OrderId == orderId).ToList(),
            };

            OrderVM.ListItems = _unitOfWork.ShippingCompany.GetAll().Select(s => new SelectListItem
            {
                Text = s.Name,
                Value = s.Name,
                Selected = OrderVM.Order.Carrier != null && OrderVM.Order.Carrier == s.Name
            });

            if (OrderVM.Order is null)
            {
                return NotFound("no data found");
            }

            return View(OrderVM);
        }

        [HttpPost]
        [Authorize(Roles = $"{StaticDetails.Role_Admin}, {StaticDetails.Role_Employee}")]
        [ValidateAntiForgeryToken]
        public IActionResult UpdateOrderDetails()
        {
#pragma warning disable CS8602 // Dereference of a possibly null reference.
            var order = _unitOfWork.Order.FindObject(o => o.Id == OrderVM.Order.Id);
#pragma warning restore CS8602 // Dereference of a possibly null reference.

#pragma warning disable CS8602 // Dereference of a possibly null reference.
            order.Username = OrderVM.Order.Username;
#pragma warning restore CS8602 // Dereference of a possibly null reference.
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
            _unitOfWork.SaveChanges();
            return RedirectToAction(nameof(Details), new { orderId = order.Id });
        }

        [HttpPost]
        [Authorize(Roles = $"{StaticDetails.Role_Admin}, {StaticDetails.Role_Employee}")]
        [ValidateAntiForgeryToken]
        public IActionResult StartProcessing()
        {
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
#pragma warning disable CS8602 // Dereference of a possibly null reference.
            _unitOfWork.Order.UpdateStatus(OrderVM.Order.Id, StaticDetails.StatusInProcess, null);
#pragma warning restore CS8602 // Dereference of a possibly null reference.
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.
            _unitOfWork.SaveChanges();
            return RedirectToAction(nameof(Details), new { orderId = OrderVM.Order.Id });
        }

        [HttpPost]
        [Authorize(Roles = $"{StaticDetails.Role_Admin}, {StaticDetails.Role_Employee}")]
        [ValidateAntiForgeryToken]
        public IActionResult ShipOrder()
        {
#pragma warning disable CS8602 // Dereference of a possibly null reference.
            var order = _unitOfWork.Order.FindObject(o => o.Id == OrderVM.Order.Id);
#pragma warning restore CS8602 // Dereference of a possibly null reference.

#pragma warning disable CS8602 // Dereference of a possibly null reference.
            order.TrackingNumber = OrderVM.Order.TrackingNumber;
#pragma warning restore CS8602 // Dereference of a possibly null reference.
            order.Carrier = OrderVM.Order.Carrier;
            order.OrderStatus = StaticDetails.StatusShipped;
            order.ShippingDate = DateTime.Now.ToString("dd/MM/yyyy hh:mm tt");

            if (order.PaymentStatus == StaticDetails.PaymentStatusDelayedPayment)
            {
                order.PaymentDueDate = DateTime.Now.AddDays(30).ToString("dd/MM/yyyy");
            }

            _unitOfWork.Order.Update(order);
            _unitOfWork.SaveChanges();
            return RedirectToAction(nameof(Details), new { orderId = OrderVM.Order.Id });
        }

        [HttpPost]
        [Authorize(Roles = $"{StaticDetails.Role_Admin}, {StaticDetails.Role_Employee}")]
        [ValidateAntiForgeryToken]
        public IActionResult CancelOrder()
        {
#pragma warning disable CS8602 // Dereference of a possibly null reference.
            var order = _unitOfWork.Order.FindObject(o => o.Id == OrderVM.Order.Id);
#pragma warning restore CS8602 // Dereference of a possibly null reference.

            if (order.PaymentStatus == StaticDetails.PaymentStatusApproved)  //individual user
            {
                var options = new RefundCreateOptions
                {
                    Reason = RefundReasons.RequestedByCustomer,
                    PaymentIntent = order.PaymentIntentId,
                };

                var service = new RefundService();
                Refund refund = service.Create(options);

                _unitOfWork.Order.UpdateStatus(order.Id, StaticDetails.StatusCancelled, StaticDetails.StatusRefunded);
            }
            else   //company user before payment
            {
                _unitOfWork.Order.UpdateStatus(order.Id, StaticDetails.StatusCancelled, StaticDetails.StatusCancelled);
            }

            _unitOfWork.Order.Update(order);
            _unitOfWork.SaveChanges();
#pragma warning disable CS8602 // Dereference of a possibly null reference.
            return RedirectToAction(nameof(Details), new { orderId = OrderVM.Order.Id });
#pragma warning restore CS8602 // Dereference of a possibly null reference.
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult PayNow()
        {
#pragma warning disable CS8602 // Dereference of a possibly null reference.
            OrderVM.OrderDetails = _unitOfWork.OrderDetail.GetAll(o => o.OrderId == OrderVM.Order.Id, x => x.Product).ToList();
#pragma warning restore CS8602 // Dereference of a possibly null reference.

            //stripe settings
#pragma warning disable CS8602 // Dereference of a possibly null reference.
            var options = new SessionCreateOptions
            {
                PaymentMethodTypes = new List<string>
                {
                    "card",
                },
                LineItems = new List<SessionLineItemOptions>(),
                Mode = "payment",
                SuccessUrl = $"https://localhost:7199/admin/orders/PaymentConfirmation?orderId={OrderVM.Order.Id}",
                CancelUrl = $"https://localhost:7199/admin/orders/Details?orderId={OrderVM.Order.Id}",
            };
#pragma warning restore CS8602 // Dereference of a possibly null reference.

            foreach (var item in OrderVM.OrderDetails)
            {
#pragma warning disable CS8602 // Dereference of a possibly null reference.
                var sessionLineItem = new SessionLineItemOptions
                {
                    PriceData = new SessionLineItemPriceDataOptions
                    {
                        UnitAmount = (int)(item.Price * 100),
                        Currency = "usd",
                        ProductData = new SessionLineItemPriceDataProductDataOptions
                        {
                            Name = item.Product.Title,
                        },
                    },
                    Quantity = item.Count,
                };
#pragma warning restore CS8602 // Dereference of a possibly null reference.
                options.LineItems.Add(sessionLineItem);
            }

            var service = new SessionService();
            Session session = service.Create(options);

            _unitOfWork.Order.UpdateStripePaymentId(OrderVM.Order.Id, session.Id, session.PaymentIntentId);
            _unitOfWork.SaveChanges();

            Response.Headers.Add("Location", session.Url);
            return new StatusCodeResult(303);
        }

        public IActionResult PaymentConfirmation(int orderId)
        {
            var order = _unitOfWork.Order.FindObject(x => x.Id == orderId);

            if (order.PaymentStatus == StaticDetails.PaymentStatusDelayedPayment)
            {
                //check the stripe status
                var service = new SessionService();
                Session session = service.Get(order.SessionId);

                if (session.PaymentStatus.ToLower() == "paid")
                {
                    order.PaymentDate = DateTime.Now.ToString("dd/MM/yyyy hh:mm tt");
#pragma warning disable CS8604 // Possible null reference argument for parameter 'orderStatus' in 'void IOrderRepository.UpdateStatus(int id, string orderStatus, string paymentStatus)'.
                    _unitOfWork.Order.UpdateStatus(orderId, order.OrderStatus, StaticDetails.PaymentStatusApproved);
#pragma warning restore CS8604 // Possible null reference argument for parameter 'orderStatus' in 'void IOrderRepository.UpdateStatus(int id, string orderStatus, string paymentStatus)'.
                    _unitOfWork.SaveChanges();
                }
            }

            return View(orderId);
        }
    }
}
