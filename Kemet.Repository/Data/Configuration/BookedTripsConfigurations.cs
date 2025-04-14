﻿using Kemet.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kemet.Repository.Data.Configuration
{
    public class BookedTripsConfigurations : IEntityTypeConfiguration<BookedTrips>
    {
        public void Configure(EntityTypeBuilder<BookedTrips> builder)
        {
            builder.Property(B => B.ReserveDate) //Dateonly mesh supported fl Efcore
                 .HasConversion(
                     v => v.ToDateTime(TimeOnly.MinValue),
                     v => DateOnly.FromDateTime(v));

            builder.HasOne(bt => bt.Customer)   // A BookedTrip belongs to one Customer
         .WithMany(c => c.BookedTrips) // A Customer can have many BookedTrips
         .HasForeignKey(bt => bt.CustomerID) // Explicitly define the FK
         .OnDelete(DeleteBehavior.Cascade); // Ensure proper delete behavior

            // Payment related configurations
            builder.Property(b => b.PaymentStatus)
                .HasMaxLength(20)
                .IsRequired();

            builder.Property(b => b.PaymentDate)
                .IsRequired(false);

            builder.Property(b => b.StripePaymentId)
                .HasMaxLength(100)
                .IsRequired(false);

            builder.Property(b => b.BookedPrice)
                .HasColumnType("decimal(18,2)")
                .IsRequired();
        }
    }
}
