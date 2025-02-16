using Kemet.Core.Entities.Identity;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kemet.Repository.Data.Configuration.Identity
{
    public class TravelAgencyConfigurations : IEntityTypeConfiguration<TravelAgency>
    {
        public void Configure(EntityTypeBuilder<TravelAgency> builder)
        {
          


            builder.HasBaseType<AppUser>();

            builder.Property(c => c.Location)
                .HasColumnType("geography")
                .IsRequired(false);

        }
    }
}
