using Kemet.Core.Entities.Images;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kemet.Repository.Data.Configuration.HomeEntitesConfigurations
{
    public class TravelAgencyPlanImagesConfigurations : IEntityTypeConfiguration<TravelAgencyPlanImages>
    {
        public void Configure(EntityTypeBuilder<TravelAgencyPlanImages> builder)
        {
            builder.HasOne(ai => ai.TravelAgencyPlan)
                 .WithMany(a => a.images)
                 .HasForeignKey(ai => ai.TravelAgencyPlanID);
        }
    }
}
