using Kemet.Core.Entities.Intersts;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kemet.Repository.Data.Configuration
{
    public class CustomerInterestConfiguration : IEntityTypeConfiguration<CustomerInterest>
    {
        public void Configure(EntityTypeBuilder<CustomerInterest> builder)
        {
            builder.HasKey(ci => new { ci.CustomerId, ci.CategoryId });

            builder.HasOne(ci => ci.Customer)
                   .WithMany(c => c.CustomerInterests)
                   .HasForeignKey(ci => ci.CustomerId);

            builder.HasOne(ci => ci.Category)
                   .WithMany(c => c.CustomerInterests)
                   .HasForeignKey(ci => ci.CategoryId);
        }
    }
}
