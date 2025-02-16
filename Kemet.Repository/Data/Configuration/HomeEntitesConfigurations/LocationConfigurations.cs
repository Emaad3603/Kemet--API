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
    public class LocationConfigurations : IEntityTypeConfiguration<Location>
    {
        public void Configure(EntityTypeBuilder<Location> builder)
        {
            builder.Property(L => L.Id).ValueGeneratedNever();
            builder.Property(l => l.Coordinates)
                 .HasColumnType("geography");
        }
    }
}
