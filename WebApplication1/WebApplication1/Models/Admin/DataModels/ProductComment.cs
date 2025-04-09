using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace WebApplication1.Models.Admin.DataModels
{
    public class ProductComment
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int CommentId { get; set; }
        [ForeignKey("Product")]
        public string ProductId { get; set; } 
        public string CustomerName { get; set; } 
        public string Content { get; set; } 
        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}
