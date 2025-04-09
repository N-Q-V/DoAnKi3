using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace WebApplication1.Models.Admin.DataModels
{
    [Table("productsColors")]
    public class ProductColors
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [ForeignKey("Product")]
        public string ProductId { get; set; }
        [Required(ErrorMessage = "Chưa chọn màu")]
        public string ProColorName { get; set; }
        public Product? Product { get; set; }
    }
}
