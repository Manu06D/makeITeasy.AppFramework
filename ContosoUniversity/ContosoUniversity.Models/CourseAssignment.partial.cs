using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ContosoUniversity.Models
{
    public partial class CourseAssignment
    {
        public object DatabaseID => new object[] { InstructorId, CourseId };
    }
}
