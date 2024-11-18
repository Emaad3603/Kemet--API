using Kemet.Core.Entities;
using Kemet.Core.Entities.Identity;
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
            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());


            base.OnModelCreating(modelBuilder);
        }
        public DbSet<OTP> OTPs { get; set; }
        public DbSet<Place> Places { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Activity> Activities { get; set; }
        public DbSet<Location> Locations { get; set; }
      
        public DbSet<TravelAgencyPlan> TravelAgencyPlans { get; set; }



    }
}
