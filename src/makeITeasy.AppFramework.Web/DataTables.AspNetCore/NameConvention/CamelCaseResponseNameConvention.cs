using System;
using System.Collections.Generic;
using System.Text;

namespace makeITeasy.AppFramework.Web.DataTables.AspNetCore.NameConvention
{
    public interface IResponseNameConvention
    {
        string Draw { get; }
        string TotalRecords { get; }
        string TotalRecordsFiltered { get; }
        string Data { get; }
        string Error { get; }
    }

    public class CamelCaseResponseNameConvention : IResponseNameConvention
    {
        public string Draw { get { return "draw"; } }
        public string TotalRecords { get { return "recordsTotal"; } }
        public string TotalRecordsFiltered { get { return "recordsFiltered"; } }
        public string Data { get { return "data"; } }
        public string Error { get { return "error"; } }
    }
}
