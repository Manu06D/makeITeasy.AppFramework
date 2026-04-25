using makeITeasy.AppFramework.Web.Models;

namespace ContosoUniversity.WebApplication.Models.Datatables
{
    public class StudentDatatable : DatatableBaseConfiguration<StudentDatatableSearchViewModel, StudentDatatableViewModel>
    {
        public StudentDatatable(string url) : base(url)
        {
            Options.PageLength = 5;
            Options.LoadOnDisplay = true;
            Options.ActivateDoubleClickOnRow = true;
            Options.EnablePaging = true;
            Options.Responsive = true;
        }
    }
}
