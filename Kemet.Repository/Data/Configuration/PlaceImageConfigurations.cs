using Kemet.Core.Entities.Images;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kemet.Repository.Data.Configuration
{
    public class PlaceImageConfigurations : IEntityTypeConfiguration<PlaceImage>
    {
        public void Configure(EntityTypeBuilder<PlaceImage> builder)
        {
            builder.HasOne(pi => pi.Place)
                   .WithMany(p => p.Images)
                   .HasForeignKey(pi => pi.PlaceId);
        }
    }
}
