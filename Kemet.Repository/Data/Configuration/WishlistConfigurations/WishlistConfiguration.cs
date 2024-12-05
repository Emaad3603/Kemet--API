using Kemet.Core.Entities.WishlistEntites;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kemet.Repository.Data.Configuration.WishlistConfigurations
{
    public class WishlistConfiguration : IEntityTypeConfiguration<Wishlist>
    {
        public void Configure(EntityTypeBuilder<Wishlist> builder)
        {
           
          
            builder.HasMany(w=>w.Activities)
                   .WithOne()
                   .OnDelete(DeleteBehavior.Cascade);
            builder.HasMany(w=>w.Places)
                   .WithOne()
                   .OnDelete(DeleteBehavior.Cascade);
            builder.HasMany(w=>w.Plans)
                   .WithOne()
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
