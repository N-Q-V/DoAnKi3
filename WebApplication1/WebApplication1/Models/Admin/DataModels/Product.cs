using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace WebApplication1.Models.Admin.DataModels
{
    [Table("products")]
    public class Product
    {
        [Key]
        [Required(ErrorMessage = "Chưa nhập mã sản phẩm")]
        [DisplayName("Mã sản phẩm")]
        public string ProductId { get; set; }
        [DisplayName("Tên sản phẩm")]
        [Required(ErrorMessage = "Chưa nhập tên sản phẩm")]
        public string ProductName { get; set; }
        [DisplayName("Giá bán (VNĐ)")]
        [Required(ErrorMessage = "Chưa nhập giá")]
        public double Price { get; set; }
        [DisplayName("Tên thương hiệu")]
        [ForeignKey("Category")]
        public int CategoryId { get; set; }
        [DisplayName("Loại sản phẩm")]
        public bool ProductType { get; set; }
        [DisplayName("Trạng thái")]
        public bool Status { get; set; }
        [DisplayName("Màn hình")]
        [Required(ErrorMessage = "Chưa nhập thông số màn hình")]
        public string Display { get; set; }
        [DisplayName("Camera trước")]
        [Required(ErrorMessage = "Chưa nhập thông số camera trước")]
        public string FrontCamera { get; set; }
        [DisplayName("Camera sau")]
        [Required(ErrorMessage = "Chưa nhập thông số camera sau")]
        public string RearCamera { get; set; }
        [DisplayName("CPU")]
        [Required(ErrorMessage = "Chưa nhập thông số CPU")]
        public string Cpu { get; set; }
        [DisplayName("Hệ điều hành")]
        [Required(ErrorMessage = "Chưa nhập thông số hệ điều hành")]
        public string System { get; set; }
        [DisplayName("Loại kết nối")]
        [Required(ErrorMessage = "Chưa nhập loại kết nối")]
        public string ConnectType { get; set; }
        [DisplayName("Bộ nhớ")]
        [Required(ErrorMessage = "Chưa nhập thông số bộ nhớ")]
        public string Memory { get; set; }
        [DisplayName("Ngày sản xuất")]
        [DataType(DataType.DateTime)]
        public DateTime CreatAt { get; set; }

        public Category? Category { get; set; }
        public ProductColors? ProductColors { get; set; }
        public ProductImages? ProductImages { get; set; }
        public ICollection<OrderItem> OrderItems { get; set; }
    }
}
