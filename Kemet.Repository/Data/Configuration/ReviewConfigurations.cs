using Kemet.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kemet.Repository.Data.Configuration
{
    public class ReviewConfigurations : IEntityTypeConfiguration<Review>
    {
        public void Configure(EntityTypeBuilder<Review> builder)
        {
            builder.HasOne(r => r.Activity)
                 .WithMany(a => a.Reviews)
                 .HasForeignKey(r => r.ActivityId)
                 .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(r => r.Place)
                .WithMany(a => a.Reviews)
                .HasForeignKey(r => r.PlaceId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(r => r.TravelAgencyPlan)
                .WithMany(a => a.Reviews)
                .HasForeignKey(r => r.TravelAgencyPlanId)
                .OnDelete(DeleteBehavior.Cascade);

        }
    }
}
