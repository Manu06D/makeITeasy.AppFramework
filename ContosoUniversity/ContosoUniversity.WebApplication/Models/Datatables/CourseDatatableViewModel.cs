using AutoMapper;

using ContosoUniversity.Models;

using makeITeasy.AppFramework.Core.Interfaces;
using makeITeasy.AppFramework.Web.Attributes;

namespace ContosoUniversity.WebApplication.Models.Datatables
{
    public class CourseDatatableViewModel : IMapFrom<Course>
    {
        [TableColumn(Name = nameof(CourseId), Title = "CourseId", IsRowId = true, Priority = 1, Visible = false)]
        public int CourseId { get; set; }

        [TableColumn(Name = nameof(Title), Title = "Title", Priority = 2)]
        public string? Title { get; set; }

        [TableColumn(Name = nameof(Credits), Title = "Credits", Priority = 2)]
        public int Credits { get; set; }

        [TableColumn(Name = nameof(EnrollmentCount), Title = "EnrollementCount", Priority = 2)]
        public int EnrollmentCount { get; set; }

        [TableColumn(Name = nameof(Edit), Title = "", Priority = 2, Sortable = false)]
        public string Edit { get; set; } = string.Empty;

        public void Mapping(Profile profile)
        {
            if (profile != null)
            {
                profile.CreateMap<Course, CourseDatatableViewModel>()
                    .ForMember(dest => dest.EnrollmentCount, src => src.MapFrom(x => x.Enrollments.Count));
            }
        }
    }
}
