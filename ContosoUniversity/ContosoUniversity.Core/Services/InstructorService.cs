using ContosoUniversity.Models;

using makeITeasy.AppFramework.Core.Services;

namespace ContosoUniversity.Core.Services
{
    public class InstructorService : BaseEntityService<Instructor>, IInstructorService
    {
    }

    internal interface IInstructorService
    {
    }
}
