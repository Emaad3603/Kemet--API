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
    public class TravelAgencyPlansConfigurations : IEntityTypeConfiguration<TravelAgencyPlan>
    {
        public void Configure(EntityTypeBuilder<TravelAgencyPlan> builder)
        {
            builder.Property(TP=>TP.PictureUrl)
                   .IsRequired();
            builder.Property(TP => TP.Price)
                   .HasColumnType("decimal(18,2)")
                   .IsRequired();
            builder.Property(TP => TP.Description)
                   .IsRequired();
            builder.Property(TP => TP.Duration)
                   .IsRequired();

            builder.Property(TP => TP.PictureUrl)
                   .IsRequired();
            builder.Property(TP => TP.PlanName)
                   .IsRequired();
            builder.Property(TP => TP.PlanAvailability)
                   .IsRequired();
            builder.HasOne(TP => TP.TravelAgency)
                .WithMany()
                .HasForeignKey(TP => TP.TravelAgencyId);


        }
    }
}
