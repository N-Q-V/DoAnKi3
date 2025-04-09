using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace WebApplication1.Models.Admin.DataModels
{
    [Table("productsImages")]
    public class ProductImages
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [ForeignKey("Product")]
        public string ProductId { get; set; }
        public string Thumb { get; set; }
        public string Images {  get; set; }
        public Product? Product { get; set; }
    }
}
