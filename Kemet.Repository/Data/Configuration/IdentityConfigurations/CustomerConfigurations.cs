using Kemet.Core.Entities.Identity;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kemet.Repository.Data.Configuration.Identity
{
    public class CustomerConfigurations : IEntityTypeConfiguration<Customer>
    {
        public void Configure(EntityTypeBuilder<Customer> builder)
        {
            builder.HasBaseType<AppUser>(); 

            builder.Property(c => c.DateOfBirth) //Dateonly mesh supported fl Efcore
                   .HasConversion(
                       v => v.ToDateTime(TimeOnly.MinValue),
                       v => DateOnly.FromDateTime(v));
            builder.HasMany(c => c.CustomerInterests)
              .WithOne(ci => ci.Customer)
              .HasForeignKey(ci => ci.CustomerId);

            builder.HasOne(C=>C.Wishlist)
                   .WithOne()
                   .OnDelete(DeleteBehavior.SetNull);

            builder.Property(c => c.Location)
                  .HasColumnType("geography")
                  .IsRequired(false);

           


        }
    }
}
