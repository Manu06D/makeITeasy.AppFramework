using ContosoUniversity.Models;
using makeITeasy.AppFramework.Core.Services;

namespace ContosoUniversity.Core.Services
{
    public class EnrollmentService : BaseEntityService<Enrollment>, IEnrollmentService
    {
    }

    internal interface IEnrollmentService
    {
    }
}
