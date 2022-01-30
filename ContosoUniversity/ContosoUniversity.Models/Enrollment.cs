using System;
using System.Collections.Generic;
using makeITeasy.AppFramework.Models;

namespace ContosoUniversity.Models
{
    public partial class Enrollment : IBaseEntity
    {
        public int EnrollmentId { get; set; }
        public int CourseId { get; set; }
        public int StudentId { get; set; }

        public virtual Course Course { get; set; }
        public virtual Student Student { get; set; }
    }
}
