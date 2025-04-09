using Microsoft.EntityFrameworkCore;
using WebApplication1.Models.Admin.DataModels;

namespace WebApplication1.Models.Admin.BusinesModels
{
    public class MobileDbContext : DbContext
    {
        public MobileDbContext(DbContextOptions<MobileDbContext> options) : base(options) { }
        public DbSet<Category>? categories { get; set; }
        public DbSet<Color>? colors { get; set; }
        public DbSet<Product>? products { get; set; }
        public DbSet<Users> users { get; set; }
        public DbSet<Order> orders { get; set; }
        public DbSet<OrderItem> orderItems { get; set; }
        public DbSet<Payment> payments { get; set; }
        public DbSet<ProductComment> productComments { get; set; }
        public DbSet<ProductColors>? productsColors { get; set; }
        public DbSet<ProductImages>? productsImages { get; set; }
    }
}
