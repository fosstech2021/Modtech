using BasePackageModule1.Models.Menu;
using BasePackageModule2.Models;
using BasePackageModule2.Models.Menu;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace BasePackageModule2.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

       
        public DbSet<AboutUs> AboutUs { get; set; }
       
        public DbSet<SliderImage> SliderImages { get; set; }
      
        public DbSet<Banner> Banners { get; set; }
        
        public DbSet<Page> Pages { get; set; }
       
        public DbSet<Post> Posts { get; set; }
        public DbSet<ContactMessage> ContactMessages { get; set; }
        public DbSet<ProductImage> ProductImages { get; set; }
        
      
        public DbSet<Contact> Contacts { get; set; }
        public DbSet<Logo> Logos { get; set; }
        public DbSet<Seo> Seo { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<SubCategory> SubCategories { get; set; }
        public DbSet<Coupon> Coupons { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Cart> Carts { get; set; }
        

        public DbSet<BusinessProfile> BusinessProfile { get; set; }

        public DbSet<Image> Images { get; set; }

        public DbSet<Menu> Menus { get; set; }

        public DbSet<MenuProduct> MenuProducts { get; set; }

        public DbSet<MenuCategory> MenuCategories { get; set; }

        public DbSet<Address> Addresses { get; set; }

        public DbSet<Order> Orders { get; set; }
        public DbSet<NewsSubscriber> NewsSubscribers { get;  set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<Category>()
                .HasMany(p => p.SubCategories)
                .WithOne(t => t.Category)
                .OnDelete(DeleteBehavior.SetNull);

            builder.Entity<Order>()
                .HasOne(a => a.User)
                .WithMany(a => a.Orders)
                .HasForeignKey(a => a.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Product>()
                .Property(p => p.CreatedAt)
                .HasDefaultValueSql("getdate()");

            builder.Entity<Post>()
                .Property(p => p.CreatedAt)
                .HasDefaultValueSql("getdate()");


            builder.Entity<Order>()
                .Property(o => o.Date)
                .HasDefaultValueSql("getdate()");
        }

    }
}
