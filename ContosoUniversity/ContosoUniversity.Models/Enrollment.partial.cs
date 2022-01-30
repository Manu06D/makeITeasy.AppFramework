using ContosoUniversity.Models.Enums;

namespace ContosoUniversity.Models
{
    public partial class Enrollment
    {
        public object DatabaseID => EnrollmentId;

        public Grade? Grade { get; set; }
    }
}
