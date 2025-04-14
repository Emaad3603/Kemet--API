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
    public class PaymentHistoryConfigurations : IEntityTypeConfiguration<PaymentHistory>
    {
        public void Configure(EntityTypeBuilder<PaymentHistory> builder)
        {
            builder.HasOne(ph => ph.BookedTrips)
                .WithMany()
                .HasForeignKey(ph => ph.BookedTripsId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Property(ph => ph.EventType)
                .HasMaxLength(50)
                .IsRequired();

            builder.Property(ph => ph.Status)
                .HasMaxLength(20)
                .IsRequired();

            builder.Property(ph => ph.Amount)
                .HasColumnType("decimal(18,2)")
                .IsRequired();

            builder.Property(ph => ph.Currency)
                .HasMaxLength(3)
                .IsRequired();

            builder.Property(ph => ph.StripePaymentId)
                .HasMaxLength(100)
                .IsRequired();

            builder.Property(ph => ph.StripeEventId)
                .HasMaxLength(100)
                .IsRequired();

            builder.Property(ph => ph.ErrorMessage)
                .HasMaxLength(500)
                .IsRequired(false);

            builder.Property(ph => ph.Metadata)
                .IsRequired(false);
        }
    }
} 