using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace makeITeasy.AppFramework.Web.Models
{
    public class DatatableOptions
    {
        [JsonPropertyName("processing")]
        public bool Processing { get; set; } = true;

        [JsonPropertyName("serverSide")]
        public bool ServerSide { get; set; } = true;

        [JsonPropertyName("responsive")]
        public bool Responsive { get; set; } = true;

        [JsonPropertyName("orderMulti")]
        public bool MultipleOrder { get; set; } = true;

        [JsonPropertyName("pageLength")]
        public int PageLength { get; set; } = 15;

        [JsonIgnore]
        public bool LoadOnDisplay { get; set; }

        [JsonPropertyName("deferLoading")]
        public int? DeferLoading
        {
            get
            {
                if (!LoadOnDisplay)
                    return 0;
                else
                    return null;
            }
        }

        public bool ActivateDoubleClickOnRow { get; set; }

        [JsonPropertyName("paging")]
        public bool EnablePaging { get; set; } = true;


        [JsonPropertyName("filter")]
        public bool EnableFilter { get; set; } = false;

        [JsonPropertyName("ordering")]
        public bool EnableOrdering { get; set; } = true;

        [JsonPropertyName("lengthChange")]
        public bool EnablePageSize { get; set; } = false;

    }
}
