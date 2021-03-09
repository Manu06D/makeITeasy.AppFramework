using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using makeITeasy.AppFramework.Web.DataTables.AspNetCore;

namespace makeITeasy.AppFramework.Web.Filters
{
    public class DatatableExceptionFilter : IExceptionFilter
    {
        private readonly ILogger<DatatableExceptionFilter> _logger;

        public DatatableExceptionFilter(ILogger<DatatableExceptionFilter> logger)
        {
            _logger = logger;
        }

        public void OnException(ExceptionContext context)
        {
            context.Result = new JsonResult(DataTablesResponse.Create(new DataTablesRequest(1,0,0,null, null), context.Exception?.Message));
        }
    }
}
