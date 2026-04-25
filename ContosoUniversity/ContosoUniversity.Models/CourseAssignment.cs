using System;
using System.Collections.Generic;
using makeITeasy.AppFramework.Models;

namespace ContosoUniversity.Models
{
    public partial class CourseAssignment : IBaseEntity
    {
        public int InstructorId { get; set; }
        public int CourseId { get; set; }

        public virtual Course Course { get; set; }
        public virtual Instructor Instructor { get; set; }
    }
}
