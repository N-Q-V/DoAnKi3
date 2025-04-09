using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using WebApplication1.Models.Admin.BusinesModels;

namespace WebApplication1.Models.Admin.DataModels
{
    [Table("categories")]
    public class Category
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int CategoryId { get; set; }

        [Required]
        public string CategoryName { get; set; }

        public bool Status { get; set; }

        public string Image { get; set; }

        public string Logo { get; set; }

        public DateTime CreatDate { get; set; }
        public ICollection<Product>? Products { get; set; }
    }
}
