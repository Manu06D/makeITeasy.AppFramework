using System;
using System.Collections.Generic;
using makeITeasy.AppFramework.Models;

namespace ContosoUniversity.Models
{
    public partial class Instructor : IBaseEntity
    {
        public Instructor()
        {
            CourseAssignments = new HashSet<CourseAssignment>();
            Departments = new HashSet<Department>();
        }

        public int Id { get; set; }
        public string LastName { get; set; }
        public string FirstName { get; set; }
        public DateTime HireDate { get; set; }

        public virtual OfficeAssignment OfficeAssignment { get; set; }
        public virtual ICollection<CourseAssignment> CourseAssignments { get; set; }
        public virtual ICollection<Department> Departments { get; set; }
    }
}
