using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace FlowerHouse.Models.Entity
{
    public class FlowerHouseContext : IdentityDbContext<HouseUser>
    {
        public FlowerHouseContext(DbContextOptions<FlowerHouseContext> options) : base(options) { }
        public DbSet<Flower> Flowers { set; get; }
        public DbSet<Tag> Tags { set; get; }
        public DbSet<FlowerTag> FlowerTags { set; get; }
        public DbSet<Assortment> Assortments { set; get; }
        public DbSet<ShoppingCart> ShoppingCarts { set; get; }
        public DbSet<CartItem> CartItems { set; get; }
        public DbSet<Order> Orders { set; get; }
        public DbSet<OrderItem> OrderItems { set; get; }
        public DbSet<Province> Provinces { set; get; }
        public DbSet<Area> Areas { set; get; }
        public DbSet<City> Cities { set; get; }
        public DbSet<Shipment> Shipments { set; get; }
        public DbSet<Purchase> Purchases { set; get; }
        public DbSet<Stock> Stocks { set; get; }
    }
}
