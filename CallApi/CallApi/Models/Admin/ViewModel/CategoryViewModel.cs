using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace CallApi.Models.Admin.ViewModel
{
    public class CategoryViewModel
    {
        public int CategoryId { get; set; }
        public string CategoryName { get; set; }
        public bool Status { get; set; }
        public string? Image { get; set; }
        public string? Logo { get; set; }
        public IFormFile? FileImage { get; set; }
        public IFormFile? FileLogo { get; set; }
        public DateTime CreatDate { get; set; }
    }
}
