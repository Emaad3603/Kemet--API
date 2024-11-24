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
    public class ActivityImageConfigurations : IEntityTypeConfiguration<ActivityImage>
    {
        public void Configure(EntityTypeBuilder<ActivityImage> builder)
        {
            builder.HasOne(ai => ai.Activity)
                   .WithMany(a => a.Images)
                   .HasForeignKey(ai => ai.ActivityId);
        }
    }
}
