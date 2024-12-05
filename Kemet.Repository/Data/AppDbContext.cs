using Kemet.Core.Entities;
using Kemet.Core.Entities.Identity;
using Kemet.Core.Entities.Images;
using Kemet.Core.Entities.Intersts;
using Kemet.Core.Entities.WishlistEntites;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Kemet.Repository.Data
{
    public class AppDbContext:IdentityDbContext<AppUser>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options):base(options) 
        {
            
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            foreach (var relationship in modelBuilder.Model.GetEntityTypes()
                                                     .SelectMany(e => e.GetForeignKeys()))
            {
                relationship.DeleteBehavior = DeleteBehavior.Restrict;
            }
            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
          

            base.OnModelCreating(modelBuilder);
        }
        public DbSet<OTP> OTPs { get; set; }
        public DbSet<Place> Places { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Activity> Activities { get; set; }
        public DbSet<Location> Locations { get; set; }
      
        public DbSet<TravelAgencyPlan> TravelAgencyPlans { get; set; }

        public DbSet<CustomerInterest> CustomerInterests { get; set; }

        public DbSet<Price> Prices { get; set; }

        public DbSet<PlaceImage> PlaceImages { get; set; }

        public DbSet<ActivityImage> ActivityImages { get; set; }

        public DbSet<Wishlist> Wishlists { get; set; }

        public DbSet<WishlistPlaces> WishlistPlaces { get; set; }

        public DbSet<WishlistActivites> WishlistActivites { get; set; }

        public DbSet<WishlistPlans> WishlistPlans { get; set;}

        public DbSet<Review> Reviews { get; set; }
    }
}
