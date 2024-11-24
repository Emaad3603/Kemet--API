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
    public class PlaceConfigurations : IEntityTypeConfiguration<Place>
    {
        public void Configure(EntityTypeBuilder<Place> builder)
        {
            builder.Property(P=>P.Name)
                   .IsRequired();

            builder.Property(P => P.CultureTips)
                  .IsRequired();

            builder.Property(P => P.Description)
                  .IsRequired();

           
            builder.Property(P => P.OpenTime)
                   .HasColumnType("Time")
                   .IsRequired();
            builder.Property(P => P.CloseTime)
                   .HasColumnType("Time")
                   .IsRequired();
            builder.Property(P => P.Duration)
                  .IsRequired();
            builder.HasOne(P => P.Price)
                   .WithOne()
                   .OnDelete(DeleteBehavior.SetNull);
            
          
                
                

        }
    }
}
