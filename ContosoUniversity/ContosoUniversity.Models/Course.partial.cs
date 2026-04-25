using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ContosoUniversity.Models
{
    public partial class Course
    {
        public object DatabaseID  => CourseId;
    }
}
