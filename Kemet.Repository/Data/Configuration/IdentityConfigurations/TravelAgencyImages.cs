using Kemet.Core.Entities.Images;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kemet.Repository.Data.Configuration.IdentityConfigurations
{
    public class TravelAgencyImagesConfigurations : IEntityTypeConfiguration<TravelAgencyImages>
    {
        public void Configure(EntityTypeBuilder<TravelAgencyImages> builder)
        {
            builder.HasOne(ai => ai.TravelAgency)
                 .WithMany(a => a.Images)
                 .HasForeignKey(ai => ai.TravelAgencyID);
        }
    }
}
