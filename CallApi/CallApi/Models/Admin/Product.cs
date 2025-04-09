using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace CallApi.Models.Admin
{
    using Microsoft.AspNetCore.Mvc.Rendering;
    using System;
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;

    public class Product
    {
        [DisplayName("Mã sản phẩm")]
        [Required(ErrorMessage = "Mã sản phẩm không được để trống.")]
        [StringLength(100, ErrorMessage = "Mã sản phẩm không được vượt quá 100 ký tự.")]
        public string ProductId { get; set; }

        [DisplayName("Tên sản phẩm")]
        [Required(ErrorMessage = "Tên sản phẩm không được để trống.")]
        [StringLength(200, ErrorMessage = "Tên sản phẩm không được vượt quá 200 ký tự.")]
        public string ProductName { get; set; }

        [DisplayName("Giá")]
        [Required(ErrorMessage = "Giá không được để trống.")]
        [Range(0, double.MaxValue, ErrorMessage = "Giá phải lớn hơn hoặc bằng 0.")]
        [DisplayFormat(DataFormatString = "{0:N0} VNĐ")]
        public double Price { get; set; }

        [DisplayName("Danh mục")]
        [Required(ErrorMessage = "Danh mục không được để trống.")]
        public int CategoryId { get; set; }

        [DisplayName("Loại sản phẩm")]
        [Required(ErrorMessage = "Loại sản phẩm không được để trống.")]
        public bool ProductType { get; set; }

        [DisplayName("Trạng thái")]
        [Required(ErrorMessage = "Trạng thái không được để trống.")]
        public bool Status { get; set; }

        [DisplayName("Hiển thị")]
        [Required(ErrorMessage = "Thông số màn hình không được để trống.")]
        [StringLength(500, ErrorMessage = "Thông tin hiển thị không được vượt quá 500 ký tự.")]
        public string Display { get; set; }

        [DisplayName("Camera trước")]
        [Required(ErrorMessage = "Thông số camera không được để trống.")]
        [StringLength(100, ErrorMessage = "Thông số camera trước không được vượt quá 100 ký tự.")]
        public string FrontCamera { get; set; }

        [DisplayName("Camera sau")]
        [Required(ErrorMessage = "Thông số camera không được để trống.")]
        [StringLength(100, ErrorMessage = "Thông số camera sau không được vượt quá 100 ký tự.")]
        public string RearCamera { get; set; }

        [DisplayName("CPU")]
        [Required(ErrorMessage = "Thông số CPU không được để trống.")]
        [StringLength(100, ErrorMessage = "Thông số CPU không được vượt quá 100 ký tự.")]
        public string Cpu { get; set; }

        [DisplayName("Hệ điều hành")]
        [Required(ErrorMessage = "Hệ điều hành không được để trống.")]
        [StringLength(100, ErrorMessage = "Tên hệ điều hành không được vượt quá 100 ký tự.")]
        public string System { get; set; }

        [DisplayName("Loại kết nối")]
        [Required(ErrorMessage = "Loại kết nối không được để trống.")]
        [StringLength(100, ErrorMessage = "Loại kết nối không được vượt quá 100 ký tự.")]
        public string ConnectType { get; set; }

        [DisplayName("Bộ nhớ")]
        [Required(ErrorMessage = "Bộ nhớ không được để trống.")]
        [StringLength(100, ErrorMessage = "Thông số bộ nhớ không được vượt quá 100 ký tự.")]
        public string Memory { get; set; }

        [DisplayName("Ngày tạo")]
        [Required(ErrorMessage = "Ngày tạo không được để trống.")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime CreatAt { get; set; }
    }

}
