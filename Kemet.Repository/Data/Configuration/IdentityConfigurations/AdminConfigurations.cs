using Kemet.Core.Entities.Identity;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kemet.Repository.Data.Configuration.IdentityConfigurations
{
    public class AdminConfigurations : IEntityTypeConfiguration<Admin>
    {
        public void Configure(EntityTypeBuilder<Admin> builder)
        {
            builder.HasBaseType<AppUser>();

            builder.Property(c => c.DateOfBirth) //Dateonly mesh supported fl Efcore
                   .HasConversion(
                       v => v.ToDateTime(TimeOnly.MinValue),
                       v => DateOnly.FromDateTime(v));

            builder.Property(c => c.Location)
                  .HasColumnType("geography")
                  .IsRequired(false);




        }
    }
}
