using System.Text.Json;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace makeITeasy.AppFramework.Web.Helpers
{
    public static class HtmlHelpers
    {
        public static HtmlString GetClientModel<TModel>(this IHtmlHelper helper) where TModel : new()
        {
            return GetClientModel(helper, new TModel());
        }

        public static HtmlString GetClientModel<TModel>(this IHtmlHelper helper, TModel model)
        {
            var options = new JsonSerializerOptions
            {
                IgnoreNullValues = true
            };

            var serializedObject = JsonSerializer.Serialize<TModel>(model, options);

            serializedObject = serializedObject.Replace("\\\"", "\\\\\"").Replace("\"", "\\\"");

            return new HtmlString($"JSON.parse(\"{serializedObject}\")");
        }
    }
}
