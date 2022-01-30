using ContosoUniversity.Models;
using makeITeasy.AppFramework.Models;

namespace ContosoUniversity.Core.Queries.CourseQueries
{
    public class BasicCourseQuery : BaseTransactionQuery<Course>
    {
        public int? ID { get; set; }

        public override void BuildQuery()
        {
            if (ID.HasValue && ID.Value > 0)
            {
                AddFunctionToCriteria(x => x.CourseId == ID);
            }
        }
    }
}
