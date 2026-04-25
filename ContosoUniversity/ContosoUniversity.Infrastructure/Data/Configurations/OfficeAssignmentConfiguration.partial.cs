using ContosoUniversity.Models;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ContosoUniversity.Infrastructure.Data.Configurations
{
    public partial class OfficeAssignmentConfiguration
    {
        partial void OnConfigurePartial(EntityTypeBuilder<OfficeAssignment> entity)
        {
            entity.HasKey(x => x.InstructorId);
        }
    }
}
