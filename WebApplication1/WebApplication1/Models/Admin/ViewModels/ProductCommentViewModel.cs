using System.ComponentModel.DataAnnotations.Schema;

namespace WebApplication1.Models.Admin.ViewModels
{
    public class ProductCommentViewModel
    {
        public string ProductId { get; set; }
        public string CustomerName { get; set; }
        public string Content { get; set; }
    }
}
