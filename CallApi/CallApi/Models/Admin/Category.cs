using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace CallApi.Models.Admin
{
    public class Category
    {
        [Display(Name = "Mã danh mục")]
        public int CategoryId { get; set; }

        [Display(Name = "Tên danh mục")]
        [Required(ErrorMessage ="Tên danh mục không được để trống")]
        public string CategoryName { get; set; }

        [Display(Name = "Trạng thái")]
        public bool Status { get; set; }

        [Display(Name = "Hình ảnh")]
        public string? Image { get; set; }

        [Display(Name = "Logo")]
        public string? Logo { get; set; }
        [Required(ErrorMessage = "Chưa chọn ảnh")]
        public IFormFile? FileImage { get; set; }
        [Required(ErrorMessage = "Chưa chọn ảnh")]
        public IFormFile? FileLogo { get; set; }

        [Display(Name = "Ngày tạo")]
        [Required(ErrorMessage = "Chưa nhập ngày")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime CreatDate { get; set; }
    }
}
