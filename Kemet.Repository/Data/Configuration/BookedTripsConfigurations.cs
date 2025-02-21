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
    public class BookedTripsConfigurations : IEntityTypeConfiguration<BookedTrips>
    {
        public void Configure(EntityTypeBuilder<BookedTrips> builder)
        {
            builder.Property(B => B.ReserveDate) //Dateonly mesh supported fl Efcore
                 .HasConversion(
                     v => v.ToDateTime(TimeOnly.MinValue),
                     v => DateOnly.FromDateTime(v));
            builder.HasMany(b => b.Customer)
                   .WithMany(c => c.BookedTrips);
                   
                   
        }
    }
}
