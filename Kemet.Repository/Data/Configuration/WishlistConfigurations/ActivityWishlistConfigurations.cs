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
    public class ActivityWishlistConfigurations : IEntityTypeConfiguration<WishlistActivites>
    {
        public void Configure(EntityTypeBuilder<WishlistActivites> builder)
        {
            builder.HasOne(a => a.Activity)
                .WithMany()
                .HasForeignKey(a => a.ActivityID);
        }
    }
}
