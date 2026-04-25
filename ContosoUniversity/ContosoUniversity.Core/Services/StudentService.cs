using ContosoUniversity.Models;

using makeITeasy.AppFramework.Core.Services;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ContosoUniversity.Core.Services
{
    public class StudentService : BaseEntityService<Student>, IStudentService
    {
    }

    internal interface IStudentService
    {
    }
}
