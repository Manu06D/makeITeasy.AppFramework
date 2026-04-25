using System;
using System.Collections.Generic;
using makeITeasy.AppFramework.Models;

namespace ContosoUniversity.Models
{
    public partial class Student : IBaseEntity
    {
        public Student()
        {
            Enrollments = new HashSet<Enrollment>();
        }

        public int Id { get; set; }
        public string LastName { get; set; }
        public string FirstName { get; set; }
        public DateTime EnrollmentDate { get; set; }

        public virtual ICollection<Enrollment> Enrollments { get; set; }
    }
}
