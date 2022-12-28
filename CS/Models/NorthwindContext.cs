using Microsoft.EntityFrameworkCore;

namespace ASPNETCoreDashboardCustomPanel {
    public partial class NorthwindContext : DbContext {
        public NorthwindContext(DbContextOptions<NorthwindContext> options)
            : base(options) {

        }
        public virtual DbSet<Product> Products { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder) {
            modelBuilder
                .Entity<Product>()
                .Property(e => e.ProductID)
                .HasConversion<string>();
        }
    }
}
