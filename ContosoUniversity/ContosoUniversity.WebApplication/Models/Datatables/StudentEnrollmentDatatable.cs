using makeITeasy.AppFramework.Web.Models;

namespace ContosoUniversity.WebApplication.Models.Datatables
{
    public class StudentEnrollmentDatatable : DatatableBaseConfiguration<StudentEnrollmentDatatableSearchViewModel, StudentEnrollmentDatatableViewModel>
    {
        public StudentEnrollmentDatatable(string url) : base(url)
        {
            Options.PageLength = 10;
            Options.LoadOnDisplay = true;
            Options.ActivateDoubleClickOnRow = true;
            Options.EnablePaging = true;
            Options.Responsive = true;
        }
    }
}
