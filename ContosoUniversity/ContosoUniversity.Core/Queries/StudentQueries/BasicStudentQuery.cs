using ContosoUniversity.Models;

using makeITeasy.AppFramework.Models;

namespace ContosoUniversity.Core.Queries.StudentQueries
{
    public class BasicinstructorQuery : BaseTransactionQuery<Student>
    {
        public int? ID { get; set; }

        public override void BuildQuery()
        {
            if (ID.HasValue && ID.Value > 0)
            {
                AddFunctionToCriteria(x => x.Id == ID);
            }
        }
    }
}