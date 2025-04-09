namespace WebApplication1.Models.Admin.ViewModels
{
    public class CategoryViewModel
    {
        public int CategoryId { get; set; }
        public string CategoryName { get; set; }
        public bool Status { get; set; }
        public IFormFile? FileImage { get; set; }
        public string? ImageOld { get; set; }

        public IFormFile? FileLogo { get; set; }
        public string? LogoOld { get; set; }
        public DateTime CreatDate { get; set; }
    }
}
