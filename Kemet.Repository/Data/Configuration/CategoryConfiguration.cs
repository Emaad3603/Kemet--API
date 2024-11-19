using Kemet.Core.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kemet.Repository.Data.Configuration
{
    public class CategoryConfiguration : IEntityTypeConfiguration<Category>
    {
        public void Configure(EntityTypeBuilder<Category> builder)
        {
            builder.HasKey(c => c.Id);

            builder.HasMany(c => c.Activity)
                   .WithOne(a => a.Category)
                   .HasForeignKey(a => a.CategoryId);

            builder.HasMany(c => c.Place)
                   .WithOne(p => p.Category)
                   .HasForeignKey(p => p.CategoryId);

            builder.HasMany(c => c.CustomerInterests)
                   .WithOne(ci => ci.Category)
                   .HasForeignKey(ci => ci.CategoryId);
        }
    }
}
