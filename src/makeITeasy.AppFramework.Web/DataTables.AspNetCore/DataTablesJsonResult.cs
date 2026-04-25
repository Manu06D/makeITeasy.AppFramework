using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc;

namespace makeITeasy.AppFramework.Web.DataTables.AspNetCore
{
    public class DataTablesJsonResult : IActionResult
    {
        /// <summary>
        /// Defines the default result content type.
        /// </summary>
        private static readonly string DefaultContentType = "application/json; charset={0}";
        /// <summary>
        /// Defines the default result enconding.
        /// </summary>
        private static readonly System.Text.Encoding DefaultContentEncoding = System.Text.Encoding.UTF8;
        /// <summary>
        /// Defines the default json request behavior.
        /// </summary>
        private static readonly bool AllowJsonThroughHttpGet = false;


        private string ContentType;
        private System.Text.Encoding ContentEncoding;
        private bool AllowGet;
        private object Data;


        public DataTablesJsonResult(IDataTablesResponse response)
            : this(response, DefaultContentType, DefaultContentEncoding, AllowJsonThroughHttpGet)
        { }

        public DataTablesJsonResult(IDataTablesResponse response, bool allowJsonThroughHttpGet)
            : this(response, DefaultContentType, DefaultContentEncoding, allowJsonThroughHttpGet)
        { }

        public DataTablesJsonResult(IDataTablesResponse response, string contentType, System.Text.Encoding contentEncoding, bool allowJsonThroughHttpGet)
        {
            Data = response;
            ContentEncoding = contentEncoding ?? System.Text.Encoding.UTF8;
            ContentType = String.Format(contentType ?? DefaultContentType, contentEncoding.WebName);
            AllowGet = allowJsonThroughHttpGet;
        }

        public async Task ExecuteResultAsync(ActionContext context)
        {
            if (!AllowGet && context.HttpContext.Request.Method.Equals("GET", StringComparison.InvariantCultureIgnoreCase))
                throw new NotSupportedException("This request has been blocked because sensitive information could be disclosed to third party web sites when this is used in a GET request. To allow GET requests, set JsonRequestBehavior to AllowGet.");

            var response = context.HttpContext.Response;

            response.ContentType = ContentType;

            if (Data != null)
            {
                var content = Data.ToString();
                var contentBytes = ContentEncoding.GetBytes(content);
                await response.Body.WriteAsync(contentBytes, 0, contentBytes.Length);
            }
        }
    }
}
