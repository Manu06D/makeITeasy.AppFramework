using ContosoUniversity.Models;
using makeITeasy.AppFramework.Models;

namespace ContosoUniversity.Core.Queries.EnrollmentQueries
{
    public class BasicEnrollmentQuery : BaseTransactionQuery<Enrollment>
    {
        public int? EnrollmentId { get; set; }
        public int? StudentId { get; set; }

        public override void BuildQuery()
        {
            if (EnrollmentId.HasValue && EnrollmentId.Value > 0)
            {
                AddFunctionToCriteria(x => x.EnrollmentId == EnrollmentId);
            }

            if (StudentId.HasValue && StudentId.Value > 0)
            {
                AddFunctionToCriteria(x => x.StudentId == StudentId);
            }
        }
    }
}
