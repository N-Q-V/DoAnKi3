using CallApi.Models.Admin;
using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace CallApi.Models.User
{
    public class CartItem
    {
        [Display(Name = "Tên Sản Phẩm")]
        public string ProductName { get; set; }

        [Display(Name = "Mã Sản Phẩm")]
        public string ProductId { get; set; }

        [Display(Name = "Số Lượng")]
        public int Quantity { get; set; }

        [Display(Name = "Giá")]
        public double Price { get; set; }

        [Display(Name = "Màu Sắc")]
        public string Color { get; set; }
        [Display(Name = "Ảnh")]
        public string? Image { get; set; }
    }
}
