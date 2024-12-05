using Kemet.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kemet.Repository.Data.Configuration.HomeEntitesConfigurations
{
    internal class ActivityConfigurations : IEntityTypeConfiguration<Activity>
    {


        public void Configure(EntityTypeBuilder<Activity> builder)
        {

            //-----------------------------------


            builder.Property(A => A.Name)
               .IsRequired();

            builder.Property(A => A.Duration)
               .IsRequired();
            builder.Property(A => A.GroupSize)
               .IsRequired();
            builder.Property(A => A.OpenTime)
                .HasColumnType("Time")
               .IsRequired();
            builder.Property(A => A.CloseTime)
                .HasColumnType("Time")
               .IsRequired();
            builder.HasOne(A => A.Place)
                .WithMany()
                .HasForeignKey(A => A.PlaceId)
                .OnDelete(DeleteBehavior.SetNull);

            builder.HasOne(A => A.Location)
               .WithMany()
               .HasForeignKey(A => A.LocationId)
                .OnDelete(DeleteBehavior.SetNull); //Disable cascade Delete;

            builder.HasOne(A => A.Price)
                   .WithOne()
                   .OnDelete(DeleteBehavior.SetNull);





        }
    }
}
