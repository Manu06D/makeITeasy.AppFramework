using makeITeasy.AppFramework.Web.Models;

namespace ContosoUniversity.WebApplication.Models.Datatables
{
    public class CourseDatatable : DatatableBaseConfiguration<CourseDatatableSearchViewModel, CourseDatatableViewModel>
    {
        public CourseDatatable(string url) : base(url)
        {
            Options.PageLength = 10;
            Options.LoadOnDisplay = true;
            Options.ActivateDoubleClickOnRow = true;
            Options.EnablePaging = true;
            Options.Responsive = true;
        }
    }
}
