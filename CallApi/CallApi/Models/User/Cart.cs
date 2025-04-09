using CallApi.Models.Admin;
using System.ComponentModel.DataAnnotations;

namespace CallApi.Models.User
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    public class Cart
    {
        [Display(Name = "Mã Đơn Hàng")]
        public int OrderId { get; set; }

        [Display(Name = "Mã Người Dùng")]
        [Required(ErrorMessage = "Mã người dùng là bắt buộc.")]
        public int UserId { get; set; }

        [Display(Name = "Họ Và Tên")]
        [Required(ErrorMessage = "Họ và tên là bắt buộc.")]
        [StringLength(100, ErrorMessage = "Họ và tên không được vượt quá 100 ký tự.")]
        public string UserName { get; set; }

        [Display(Name = "Tổng Số Tiền")]
        [Required(ErrorMessage = "Tổng số tiền là bắt buộc.")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Tổng số tiền phải lớn hơn 0.")]
        public double TotalAmount { get; set; }

        [Display(Name = "Thanh Toán")]
        [Required(ErrorMessage = "Trạng thái thanh toán là bắt buộc.")]
        public string PaymentStatus { get; set; }

        [Display(Name = "Địa Chỉ Giao Hàng")]
        [Required(ErrorMessage = "Địa chỉ giao hàng là bắt buộc.")]
        [StringLength(250, ErrorMessage = "Địa chỉ không được vượt quá 250 ký tự.")]
        public string ShippingAddress { get; set; }

        [Display(Name = "Số điện thoại")]
        [Required(ErrorMessage = "Số điện thoại là bắt buộc.")]
        [Phone(ErrorMessage = "Số điện thoại không hợp lệ.")]
        public string Phone { get; set; }

        [Display(Name = "Email")]
        [Required(ErrorMessage = "Email là bắt buộc.")]
        [EmailAddress(ErrorMessage = "Địa chỉ email không hợp lệ.")]
        public string Email { get; set; }

        [Display(Name = "Ngày Đặt Hàng")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        [Required(ErrorMessage = "Ngày đặt hàng là bắt buộc.")]
        public DateTime OrderDate { get; set; }

        [Display(Name = "Trạng thái đơn hàng")]
        public string Status { get; set; }

        public string? Color { get; set; }

        public List<CartItem> Items { get; set; } = new List<CartItem>();

        public void AddItem(Product product, int quantity, string color, ProductImage image)
        {
            var existingItem = Items.Find(p => p.ProductId == product.ProductId);
            if (existingItem != null)
            {
                existingItem.Quantity += quantity;
            }
            else
            {
                Items.Add(new CartItem { ProductName = product.ProductName, Quantity = quantity, Price = product.Price, ProductId = product.ProductId, Color = color, Image = image.Thumb });
            }
        }

        public double TotalPrice()
        {
            double total = 0;
            foreach (var item in Items)
            {
                total += item.Price * item.Quantity;
            }
            return total;
        }
    }

}
