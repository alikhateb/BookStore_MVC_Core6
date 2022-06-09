using Microsoft.AspNetCore.Mvc.Rendering;

namespace BookStore.Models.ViewModels
{
    public class OrderVM
    {
        public Order? Order { get; set; }
        public IEnumerable<OrderDetail>? OrderDetails { get; set; }
        public IEnumerable<SelectListItem>? ListItems { get; set; }
    }
}
