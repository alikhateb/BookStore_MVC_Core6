﻿namespace BookStore.App.Areas.Customer.Controllers
{
    [Area("Customer")]
    [Authorize]
    public class CartController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IEmailSender _emailSender;
        private readonly UserManager<AppUser> _userManager;

        [BindProperty]
        public ShoppingCartVm ShoppingCartVm { get; set; }
        public AppUser AppUser { get; set; }

#pragma warning disable CS8618 // Non-nullable property 'AppUser' must contain a non-null value when exiting constructor. Consider declaring the property as nullable.
#pragma warning disable CS8618 // Non-nullable property 'ShoppingCartVm' must contain a non-null value when exiting constructor. Consider declaring the property as nullable.
        public CartController(IUnitOfWork _unitOfWork, UserManager<AppUser> _userManager, IEmailSender _emailSender)
#pragma warning restore CS8618 // Non-nullable property 'ShoppingCartVm' must contain a non-null value when exiting constructor. Consider declaring the property as nullable.
#pragma warning restore CS8618 // Non-nullable property 'AppUser' must contain a non-null value when exiting constructor. Consider declaring the property as nullable.
        {
            this._unitOfWork = _unitOfWork;
            this._userManager = _userManager;
            this._emailSender = _emailSender;
        }

        public IActionResult Index()
        {
            AppUser = _userManager.GetUserAsync(User).Result;
            ShoppingCartVm = LoadCartItems(AppUser);
            return View(ShoppingCartVm);
        }

        public IActionResult SubmitOrder()
        {
            AppUser = _userManager.GetUserAsync(User).Result;

            ShoppingCartVm = LoadCartItems(AppUser);

#pragma warning disable CS8604 // Possible null reference argument for parameter 'source' in 'bool Enumerable.Any<ShoppingCartItem>(IEnumerable<ShoppingCartItem> source)'.
            if (!ShoppingCartVm.CartItems.Any())
            {
                return RedirectToAction("index", "home");
            }
#pragma warning restore CS8604 // Possible null reference argument for parameter 'source' in 'bool Enumerable.Any<ShoppingCartItem>(IEnumerable<ShoppingCartItem> source)'.

#pragma warning disable CS8602 // Dereference of a possibly null reference.
            ShoppingCartVm.Order.AppUserId = AppUser.Id;
#pragma warning restore CS8602 // Dereference of a possibly null reference.
            ShoppingCartVm.Order.Username = AppUser.UserName;
            ShoppingCartVm.Order.Email = AppUser.Email;
            ShoppingCartVm.Order.Address = AppUser.Address;
            ShoppingCartVm.Order.City = AppUser.City;
            ShoppingCartVm.Order.State = AppUser.State;
            ShoppingCartVm.Order.PostalCode = AppUser.PostalCode;
            ShoppingCartVm.Order.PhoneNumber = AppUser.PhoneNumber;

            return View(ShoppingCartVm);
        }

        [HttpPost]
        [ActionName(nameof(SubmitOrder))]
        [ValidateAntiForgeryToken]
        public IActionResult SubmitOrderPost()
        {
            AppUser = _userManager.GetUserAsync(User).Result;

            //ShoppingCartVm.CartItems = _unitOfWork.ShoppingCartItem.GetAll(x => x.AppUserId == AppUser.Id, p => p.Product);
            ShoppingCartVm.CartItems = _unitOfWork.ShoppingCartItem.GetAll(x => x.AppUserId == AppUser.Id);

            foreach (var item in ShoppingCartVm.CartItems)
            {
#pragma warning disable CS8602 // Dereference of a possibly null reference.
                item.Price = GetPriceBasedOnQuality(item.Count, item.Product.Price, item.Product.Price50, item.Product.Price100);
#pragma warning restore CS8602 // Dereference of a possibly null reference.
#pragma warning disable CS8602 // Dereference of a possibly null reference.
                ShoppingCartVm.Order.TotalPrice += item.Price * item.Count;
#pragma warning restore CS8602 // Dereference of a possibly null reference.
            }

            if (!ModelState.IsValid)
            {
                return View(ShoppingCartVm);
            }

            var order = _unitOfWork.Order
                .FindLastOrder(o => o.AppUserId == AppUser.Id && o.PaymentStatus == StaticDetails.PaymentStatusPending);

            //check if something wrong happend during payment or user leave payment page so that application dont insert duplicated data to database
            if (order is null)
            {
                if (AppUser.CompanyId.GetValueOrDefault() == 0)  //for individual users
                {
#pragma warning disable CS8602 // Dereference of a possibly null reference.
                    ShoppingCartVm.Order.OrderStatus = StaticDetails.StatusPending;
#pragma warning restore CS8602 // Dereference of a possibly null reference.
                    ShoppingCartVm.Order.PaymentStatus = StaticDetails.PaymentStatusPending;
                }
                else  //for company users
                {
#pragma warning disable CS8602 // Dereference of a possibly null reference.
                    ShoppingCartVm.Order.OrderStatus = StaticDetails.StatusApproved;
#pragma warning restore CS8602 // Dereference of a possibly null reference.
                    ShoppingCartVm.Order.PaymentStatus = StaticDetails.PaymentStatusDelayedPayment;
                }

                _unitOfWork.Order.Add(ShoppingCartVm.Order);
                _unitOfWork.SaveChanges();

                IEnumerable<OrderDetail> orderDetailList = ShoppingCartVm.CartItems.Select(o => new OrderDetail
                {
                    ProductId = o.ProductId,
                    Count = o.Count,
                    Price = o.Price,
                    OrderId = ShoppingCartVm.Order.Id
                });

                _unitOfWork.OrderDetail.AddRange(orderDetailList);
                _unitOfWork.SaveChanges();

                if (AppUser.CompanyId.GetValueOrDefault() == 0)  //for individual users
                {
                    //stripe settings
                    var options = new SessionCreateOptions
                    {
                        PaymentMethodTypes = new List<string>
                        {
                            "card",
                        },
                        LineItems = new List<SessionLineItemOptions>(),
                        Mode = "payment",
                        SuccessUrl = $"https://localhost:7199/customer/cart/OrderConfirmation?id={ShoppingCartVm.Order.Id}",
                        CancelUrl = $"https://localhost:7199/customer/cart/index",
                    };

                    foreach (var item in ShoppingCartVm.CartItems)
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

                    _unitOfWork.Order.UpdateStripePaymentId(ShoppingCartVm.Order.Id, session.Id, session.PaymentIntentId);
                    _unitOfWork.SaveChanges();

                    Response.Headers.Add("Location", session.Url);
                    return new StatusCodeResult(303);
                }
                else  //for company users
                {
                    return RedirectToAction(nameof(OrderConfirmation), new { id = ShoppingCartVm.Order.Id });
                }

            }
            else
            {
                IEnumerable<OrderDetail> existstingOrderDetailList = _unitOfWork.OrderDetail.GetAll(o => o.OrderId == order.Id);
                _unitOfWork.OrderDetail.RemoveRange(existstingOrderDetailList);
                _unitOfWork.SaveChanges();

                IEnumerable<OrderDetail> orderDetailList = ShoppingCartVm.CartItems.Select(o => new OrderDetail
                {
                    ProductId = o.ProductId,
                    Count = o.Count,
                    Price = o.Price,
                    OrderId = order.Id
                });

                _unitOfWork.OrderDetail.AddRange(orderDetailList);
                _unitOfWork.SaveChanges();

                if (AppUser.CompanyId.GetValueOrDefault() == 0)  //for individual users
                {
                    //stripe settings
                    var options = new SessionCreateOptions
                    {
                        PaymentMethodTypes = new List<string>
                        {
                            "card",
                        },
                        LineItems = new List<SessionLineItemOptions>(),
                        Mode = "payment",
                        SuccessUrl = $"https://localhost:7199/customer/cart/OrderConfirmation?id={order.Id}",
                        CancelUrl = $"https://localhost:7199/customer/cart/index",
                    };

                    foreach (var item in ShoppingCartVm.CartItems)
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

                    _unitOfWork.Order.UpdateStripePaymentId(order.Id, session.Id, session.PaymentIntentId);
                    _unitOfWork.SaveChanges();

                    Response.Headers.Add("Location", session.Url);
                    return new StatusCodeResult(303);
                }
                //for company users
                return RedirectToAction(nameof(OrderConfirmation), new { id = order.Id });
            }
        }

        public IActionResult OrderConfirmation(int id)
        {
            Order order = _unitOfWork.Order.FindObject(x => x.Id == id);

            if (order.PaymentStatus != StaticDetails.PaymentStatusDelayedPayment)  //for individual user
            {
                //check the stripe status
                var service = new SessionService();
                Session session = service.Get(order.SessionId);

                if (session.PaymentStatus.ToLower() == "paid")
                {
                    order.PaymentDate = DateTime.Now.ToString("dd/MM/yyyy hh:mm tt");
                    _unitOfWork.Order.UpdateStatus(id, StaticDetails.StatusApproved, StaticDetails.PaymentStatusApproved);
                    _unitOfWork.SaveChanges();
                }
            }

            _emailSender.SendEmailAsync(order.Email, "New Order From Book Store", "<p>Congratulations New Order Is Created Successfully</p>");

            IEnumerable<ShoppingCartItem> shoppingCartItems = _unitOfWork.ShoppingCartItem.GetAll(x => x.AppUserId == order.AppUserId);
            _unitOfWork.ShoppingCartItem.RemoveRange(shoppingCartItems);
            _unitOfWork.SaveChanges();

            HttpContext.Session.Clear();

            return View(id);
        }

        public IActionResult Plus(int itemId)
        {
            var cartItem = _unitOfWork.ShoppingCartItem.FindObject(x => x.Id == itemId);
            _unitOfWork.ShoppingCartItem.IncrementCount(cartItem, 1);
            _unitOfWork.ShoppingCartItem.Update(cartItem);
            _unitOfWork.SaveChanges();
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Minus(int itemId)
        {
            var cartItem = _unitOfWork.ShoppingCartItem.FindObject(x => x.Id == itemId);
            _unitOfWork.ShoppingCartItem.DecrementCount(cartItem, 1);
            _unitOfWork.ShoppingCartItem.Update(cartItem);
            _unitOfWork.SaveChanges();
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Delete(int itemId)
        {
            var cartItem = _unitOfWork.ShoppingCartItem.FindObject(x => x.Id == itemId);
            _unitOfWork.ShoppingCartItem.Remove(cartItem);
            _unitOfWork.SaveChanges();

            HttpContext.Session.SetInt32(StaticDetails.SessionCart,
                _unitOfWork.ShoppingCartItem.GetAll(s => s.AppUserId == cartItem.AppUserId).Count());

            return RedirectToAction(nameof(Index));
        }

        private ShoppingCartVm LoadCartItems(AppUser user)
        {
            ShoppingCartVm = new ShoppingCartVm
            {
                //CartItems = _unitOfWork.ShoppingCartItem.GetAll(x => x.AppUserId == user.Id, p => p.Product),
                CartItems = _unitOfWork.ShoppingCartItem.GetAll(x => x.AppUserId == user.Id),
                Order = new Order()
            };

            foreach (var item in ShoppingCartVm.CartItems)
            {
#pragma warning disable CS8602 // Dereference of a possibly null reference.
                item.Price = GetPriceBasedOnQuality(item.Count, item.Product.Price, item.Product.Price50, item.Product.Price100);
#pragma warning restore CS8602 // Dereference of a possibly null reference.
                ShoppingCartVm.Order.TotalPrice += item.Price * item.Count;
            }

            return ShoppingCartVm;
        }

        private double GetPriceBasedOnQuality(int quantity, double price, double price50, double price100)
        {
            if (quantity <= 50)
            {
                return price;
            }
            else if (quantity >= 51 && quantity <= 100)
            {
                return price50;
            }
            else
            {
                return price100;
            }
        }
    }
}
