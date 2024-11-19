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
    public class PriceConfigurations : IEntityTypeConfiguration<Price>
    {
        public void Configure(EntityTypeBuilder<Price> builder)
        {
            builder.Property(P => P.EgyptianAdult)
                   .HasColumnType("decimal(18,2)");

            builder.Property(P => P.EgyptianStudent)
                   .HasColumnType("decimal(18,2)");

            builder.Property(P => P.TouristAdult)
                   .HasColumnType("decimal(18,2)");

            builder.Property(P => P.TouristStudent)
                   .HasColumnType("decimal(18,2)");
        }
    }
}
